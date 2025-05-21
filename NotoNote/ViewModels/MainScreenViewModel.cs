using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using NotoNote.Utilities;
using Stateless;
using System.Diagnostics;
using System.Windows;

namespace NotoNote.ViewModels;
public partial class MainScreenViewModel : ObservableObject
{
    private enum State
    {
        Idle,
        Recording,
        Processing,
        Settings
    }

    private enum Trigger
    {
        StartRecording,
        StopRecording,
        CompletedProcessing,
        Cancel,
        Retry,
        EnterSettings,
        ExitSettings,
    }

    private readonly Hotkey CancelHotkey = new(Keys.Escape, Keys.None);

    private readonly StateMachine<State, Trigger> _stateMachine;
    private readonly IClipBoardService _clipboardService;
    private readonly IWindowService _windowService;
    private readonly IHotkeyService _hotKeyService;
    private readonly IProfileRepository _profilesRepository;
    private readonly IHotkeyRepository _hotkeyRepository;
    private readonly IAudioService _audioService;
    private readonly ITranscriptionServiceFactory _transcriptionServiceFactory;
    private readonly IChatServiceFactory _chatServiceFactory;
    private readonly ITranscriptionModelProvider _transcriptionModelProvider;
    private readonly IChatModelProvider _chatModelProvider;
    private readonly IApiKeyRepository _apiKeyRepository;

    [ObservableProperty] private Profile _selectedProfile;
    [ObservableProperty] private ObservableCollection2<Profile> _profiles;
    [ObservableProperty] private bool _isIdle;
    [ObservableProperty] private bool _isRecording;
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private bool _isSettings;
    [ObservableProperty] private string _processedText = "";
    [ObservableProperty] private string _activationHotkeyText = "";
    [ObservableProperty] private string _stopRecordingHotkeyText = "";
    [ObservableProperty] private string _recordingMessage = "Recording...";
    [ObservableProperty] private bool _enableRecordingAnimation = true;

    private TranscriptText? _transcriptText;
    private WaveFilePath? _waveFilePath;
    private CancellationTokenSource _processingCtx = new();

    /// <summary>
    /// ProcessedTextをもち再処理が可能か
    /// </summary>
    public bool HasProcessedText => !string.IsNullOrEmpty(ProcessedText);

    public MainScreenViewModel(
        IHotkeyService hotKey,
        IProfileRepository profiles,
        IAudioService audio,
        ITranscriptionServiceFactory transcription,
        IChatServiceFactory chat,
        IWindowService window,
        IClipBoardService clipboard,
        IHotkeyRepository hotkeyRepo,
        ITranscriptionModelProvider transcriptionModelProvider,
        IChatModelProvider chatModelProvider,
        IApiKeyRepository apiKeyRepository)
    {
        // initialize fields
        _hotKeyService = hotKey;
        _profilesRepository = profiles;
        _audioService = audio;
        _transcriptionServiceFactory = transcription;
        _chatServiceFactory = chat;
        _windowService = window;
        _clipboardService = clipboard;
        _hotkeyRepository = hotkeyRepo;
        _transcriptionModelProvider = transcriptionModelProvider;
        _chatModelProvider = chatModelProvider;
        _apiKeyRepository = apiKeyRepository;

        // Set profiles
        Profiles = new(_profilesRepository.GetAll());
        var activeId = _profilesRepository.GetActiveProfileId();
        _selectedProfile = Profiles.FirstOrDefault(x => x.Id == activeId) ?? Profiles[0];

        // Setup state machine
        _stateMachine = new(State.Idle);
        ConfigureStateMachine();
        SetStateFlags(_stateMachine.State);
        OnEntryIdle();
        UpdateHotkeyTextBlock();
    }

    partial void OnSelectedProfileChanged(Profile value)
    {
        if (value == null) return;
        _profilesRepository.SetActiveProfile(value.Id);
    }

    private void ConfigureStateMachine()
    {
        _stateMachine.Configure(State.Idle)
            .OnEntry(OnEntryIdle)
            .Permit(Trigger.StartRecording, State.Recording)
            .Permit(Trigger.EnterSettings, State.Settings)
            .PermitIf(Trigger.Retry, State.Processing, () => HasProcessedText);

        _stateMachine.Configure(State.Recording)
            .OnEntry(OnEntryRecording)
            .OnExitAsync(OnExitRecordingAsync)
            .Permit(Trigger.StopRecording, State.Processing)
            .Permit(Trigger.Cancel, State.Idle);

        _stateMachine.Configure(State.Processing)
            .OnEntryAsync(OnEntryProcessingAsync)
            .OnExit(OnExitProcessing)
            .Permit(Trigger.CompletedProcessing, State.Idle)
            .Permit(Trigger.Cancel, State.Idle);

        _stateMachine.Configure(State.Settings)
            .OnEntry(OnEntrySettings)
            .OnExit(OnExitSettings)
            .Permit(Trigger.ExitSettings, State.Idle);

        _stateMachine.OnTransitioned(t => SetStateFlags(t.Destination));
    }

    private void UpdateHotkeyTextBlock()
    {
        var activationHotkey = _hotkeyRepository.Get(HotkeyPurpose.Activation);
        var toggleProfileHotkey = _hotkeyRepository.Get(HotkeyPurpose.ToggleProfile);
        ActivationHotkeyText = $"{activationHotkey}: Start recording\n{toggleProfileHotkey}: Toggle profiles";
        StopRecordingHotkeyText = $"{activationHotkey}: Stop recording\nESC: Cancel";
    }

    private void OnEntryIdle()
    {
        Debug.WriteLine("[Idle] Waiting for input.");

        OnPropertyChanged(nameof(HasProcessedText));

        // Register hotkeys
        _hotKeyService.UnregisterAllHotkeys();
        var activationHotkey = _hotkeyRepository.Get(HotkeyPurpose.Activation);
        _hotKeyService.RegisterHotkey(activationHotkey, HandleActivationHotkeyAsync);
        var toggleProfileHotkey = _hotkeyRepository.Get(HotkeyPurpose.ToggleProfile);
        _hotKeyService.RegisterHotkey(toggleProfileHotkey, HandleProfileToggleHotkey);
    }
    private void OnEntryRecording()
    {
        Debug.WriteLine("[Recording] Start.");

        try
        {
            // API Key check
            if (_chatModelProvider.Get(SelectedProfile.ChatModelId) is OpenAiCompatibleChatModel ocm && ocm.RequireApiKey)
            {
                if (_apiKeyRepository.Get(ocm.ApiSource.ApiSource) == null)
                {
                    throw new InvalidOperationException($"{ocm.ApiSource} API Key not found");
                }
            }
            if (_transcriptionModelProvider.Get(SelectedProfile.TranscriptionModelId) is OpenAiCompatibleTranscribeModel otm && otm.RequireApiKey)
            {
                if (_apiKeyRepository.Get(otm.ApiSource.ApiSource) == null)
                {
                    throw new InvalidOperationException($"{otm.ApiSource} API Key not found");
                }
            }

            // Register hotkeys
            _hotKeyService.UnregisterAllHotkeys();
            var activationHotkey = _hotkeyRepository.Get(HotkeyPurpose.Activation);
            _hotKeyService.RegisterHotkey(activationHotkey, HandleActivationHotkeyAsync);
            _hotKeyService.RegisterHotkey(CancelHotkey, HandleCancelHotkeyAsync);

            // Window control
            _windowService.SetTopmost(true);

            // Start recording
            _audioService.StartRecording(NotifyRecordingTimeout);
            ResetRecordingState();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            System.Windows.MessageBox.Show(ex.Message, "Noto note", MessageBoxButton.OK, MessageBoxImage.Error);
            _stateMachine.Fire(Trigger.Cancel);
        }
    }

    private async Task OnExitRecordingAsync()
    {
        Debug.WriteLine("[Recording] Stop.");

        _hotKeyService.UnregisterAllHotkeys();

        _windowService.SetTopmost(false);
        _waveFilePath = await _audioService.StopRecordingAsync();
    }

    private async Task OnEntryProcessingAsync()
    {
        Debug.WriteLine("[Processing] Start.");

        // Register hotkeys
        _hotKeyService.UnregisterAllHotkeys();
        _hotKeyService.RegisterHotkey(CancelHotkey, HandleCancelHotkeyAsync);

        _processingCtx = new CancellationTokenSource();
        var ct = _processingCtx.Token;
        try
        {
            var transcriptionModel = _transcriptionModelProvider.Get(SelectedProfile.TranscriptionModelId) ?? throw new InvalidOperationException($"Transcription model not found: {SelectedProfile.TranscriptionModelId}");
            var transcriptionService = _transcriptionServiceFactory.Create(transcriptionModel);
            var chatModel = _chatModelProvider.Get(SelectedProfile.ChatModelId) ?? throw new InvalidOperationException($"Chat model not found: {SelectedProfile.ChatModelId}");
            var chatService = _chatServiceFactory.Create(chatModel);

            Debug.WriteLine($"[Processing] TranscriptionModel: {transcriptionModel.DisplayName.Value}, ChatModel: {chatModel.DisplayName.Value}");

            // transcriptTextをワイプしてない場合はTranscribeをスキップする。API費用節約と高速化のため。
            if (_transcriptText == null)
            {
                if (_waveFilePath == null) throw new InvalidOperationException();
                _transcriptText = await transcriptionService.TranscribeAsync(_waveFilePath);
            }

            var chatResponseText = await chatService.CompleteChatAsync(SelectedProfile.SystemPrompt, _transcriptText, ct);

            ProcessedText = chatResponseText.Value;

            _clipboardService.Paste(ProcessedText);
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "Noto note");
        }
        _stateMachine.Fire(Trigger.CompletedProcessing);
    }

    private void OnExitProcessing()
    {
        Debug.WriteLine("[Processing] Stop.");

        // hotkey
        _hotKeyService.UnregisterAllHotkeys();

        _processingCtx?.Cancel();
        _processingCtx?.Dispose();
    }

    private void OnEntrySettings()
    {
        Debug.WriteLine("[Settings] Open.");

        // 設定画面で実際にキーコンビネーションを入力して設定する際に干渉するので、Hotkeyを無効化する
        _hotKeyService.UnregisterAllHotkeys();
    }

    private void OnExitSettings()
    {
        Debug.WriteLine("[Settings] Close.");

        Profiles.Clear();
        Profiles.AddRange(_profilesRepository.GetAll());
        var activeId = _profilesRepository.GetActiveProfileId();
        SelectedProfile = Profiles.First(x => x.Id == activeId);
    }

    private void SetStateFlags(State state)
    {
        IsIdle = state == State.Idle;
        IsRecording = state == State.Recording;
        IsProcessing = state == State.Processing;
        IsSettings = state == State.Settings;
    }

    private async Task HandleActivationHotkeyAsync()
    {
        switch (_stateMachine.State)
        {
            case State.Idle:
                await _stateMachine.FireAsync(Trigger.StartRecording);
                break;
            case State.Recording:
                await _stateMachine.FireAsync(Trigger.StopRecording);
                break;
            default:
                // nothing to do
                break;
        }
    }

    private async Task HandleCancelHotkeyAsync()
    {
        switch (_stateMachine.State)
        {
            case State.Recording:
                await _stateMachine.FireAsync(Trigger.Cancel);
                break;
            case State.Processing:
                _processingCtx.Cancel();
                await _stateMachine.FireAsync(Trigger.Cancel);
                break;
            default:
                // nothing to do
                break;
        }
    }

    private void HandleProfileToggleHotkey()
    {
        switch (_stateMachine.State)
        {
            case State.Idle:
                _windowService.Activate();
                var profiles = Profiles.ToList();
                var index = profiles.IndexOf(SelectedProfile);
                var profilesCount = profiles.Count;
                SelectedProfile = profiles[(index + 1) % profilesCount];
                break;
            default:
                break;
        }
    }

    private void ResetRecordingState()
    {
        _transcriptText = null;
        _waveFilePath = null;
        ProcessedText = "";
        RecordingMessage = "Recording...";
        EnableRecordingAnimation = true;
    }

    private void NotifyRecordingTimeout()
    {
        RecordingMessage = "Recording paused (timeout)";
        EnableRecordingAnimation = false;
    }

    [RelayCommand]
    public void StartRecording() => _stateMachine.Fire(Trigger.StartRecording);

    [RelayCommand]
    public void Cancel() => _stateMachine.Fire(Trigger.Cancel);

    [RelayCommand]
    public async Task Retry() => await _stateMachine.FireAsync(Trigger.Retry);

    [RelayCommand]
    public void EnterSettings() => _stateMachine.Fire(Trigger.EnterSettings);

    [RelayCommand]
    public void ExitSettings() => _stateMachine.Fire(Trigger.ExitSettings);


}