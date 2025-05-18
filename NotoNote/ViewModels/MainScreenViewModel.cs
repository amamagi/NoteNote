using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.DataStore;
using NotoNote.Models;
using Stateless;
using System.IO;

namespace NotoNote.ViewModels;
public partial class MainScreenViewModel : ObservableObject
{
    public enum State
    {
        Idle,
        Recording,
        Processing,
        Settings
    }

    public enum Trigger
    {
        StartRecording,
        StopRecording,
        CompletedProcessing,
        Cancel,
        Retry,
        EnterSettings,
        ExitSettings,
    }

    private readonly Hotkey ActivationHotkey = new(Keys.Space, Keys.Shift | Keys.Control);
    private readonly Hotkey ToggleProfileHotkey = new(Keys.S, Keys.Shift | Keys.Control);
    private readonly Hotkey CancelHotkey = new(Keys.Escape, Keys.None);

    private readonly IClipBoardService _clipboard;
    private readonly IWindowService _window;
    private readonly IHotkeyService _hotKey;
    private readonly IProfileRepository _profiles;
    private readonly IAudioService _audio;
    private readonly ITranscriptionAiServiceFactory _transcriptionFactory;
    private readonly IChatAiServiceFactory _chatFactory;
    private readonly StateMachine<State, Trigger> _machine;

    [ObservableProperty] private bool _isIdle;
    [ObservableProperty] private bool _isRecording;
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private bool _isSettings;
    [ObservableProperty] private Profile _selectedProfile;
    [ObservableProperty] private string _processedText = "";
    [ObservableProperty] private string _activationHotkeyText = "";
    [ObservableProperty] private string _stopRecordingHotkeyText = "";

    private ITranscriptionAiService? _transcriptionAiService;
    private IChatAiService? _chatAiService;

    private TranscriptText? _transcriptText;
    private WaveFilePath? _waveFilePath;
    private CancellationTokenSource _processingCtx = new();

    public List<Profile> Profiles { get; set; }

    /// <summary>
    /// ProcessedTextをもち再処理が可能か
    /// </summary>
    public bool HasProcessedText => !string.IsNullOrEmpty(ProcessedText);

    public MainScreenViewModel(
        IHotkeyService hotKey,
        IProfileRepository profiles,
        IAudioService audio,
        ITranscriptionAiServiceFactory transcription,
        IChatAiServiceFactory chat,
        IWindowService window,
        IClipBoardService clipboard)
    {
        _hotKey = hotKey;
        _profiles = profiles;
        _audio = audio;
        _transcriptionFactory = transcription;
        _chatFactory = chat;
        _window = window;
        _clipboard = clipboard;

        Profiles = _profiles.GetAll();

        var activeId = _profiles.GetActiveProfileId();
        _selectedProfile = Profiles.FirstOrDefault(x => x.Id == activeId) ?? Profiles[0];

        //_machine = new(State.Idle);
        _machine = new(State.Settings);
        ConfigureStateMachine();

        _hotKey.RegisterHotkey(ActivationHotkey, HandleActivationHotkey);
        _hotKey.RegisterHotkey(CancelHotkey, HandleCancelHotkey);
        _hotKey.RegisterHotkey(ToggleProfileHotkey, HandleProfileToggleHotkey);

        ActivationHotkeyText = GetHotkeyText(ActivationHotkey) + " : Start recording\n" + GetHotkeyText(ToggleProfileHotkey) + " : Toggle profiles";
        StopRecordingHotkeyText = GetHotkeyText(ActivationHotkey) + " : Stop recording\nESC: Cancel";
        _clipboard = clipboard;
    }

    partial void OnSelectedProfileChanged(Profile value)
    {
        _profiles.SetActiveProfile(value.Id);
    }

    private string GetHotkeyText(Hotkey hotkey)
    {
        return hotkey.Modifiers.ToString().Replace(", ", "+") + "+" + hotkey.Key.ToString();
    }

    private void ConfigureStateMachine()
    {
        _machine.Configure(State.Idle)
            .OnEntry(OnEntryIdle)
            .Permit(Trigger.StartRecording, State.Recording)
            .Permit(Trigger.EnterSettings, State.Settings)
            .PermitIf(Trigger.Retry, State.Processing, () => HasProcessedText);

        _machine.Configure(State.Recording)
            .OnEntry(OnEntryRecording)
            .OnExitAsync(OnExitRecordingAsync)
            .Permit(Trigger.StopRecording, State.Processing)
            .Permit(Trigger.Cancel, State.Idle);

        _machine.Configure(State.Processing)
            .OnEntryAsync(OnEntryProcessingAsync)
            .OnExit(OnExitProcessing)
            .Permit(Trigger.CompletedProcessing, State.Idle)
            .Permit(Trigger.Cancel, State.Idle);

        _machine.Configure(State.Settings)
            .OnExit(OnExitSettings)
            .Permit(Trigger.ExitSettings, State.Idle);

        // _machine.StateをViewフラグに反映
        SetStateFlags(_machine.State);
        _machine.OnTransitioned(t => SetStateFlags(t.Destination));
    }

    private void OnEntryIdle()
    {
        OnPropertyChanged(nameof(HasProcessedText));
    }

    private void Reset()
    {
        _transcriptText = null;
        _waveFilePath = null;
        ProcessedText = "";
    }

    private void OnEntryRecording()
    {
        _audio.StartRecording();
        Reset();
    }

    private async Task OnExitRecordingAsync()
    {
        _waveFilePath = await _audio.StopRecordingAsync();
    }

    private async Task OnEntryProcessingAsync()
    {
        _processingCtx = new CancellationTokenSource();
        var ct = _processingCtx.Token;
        try
        {
            _transcriptionAiService = _transcriptionFactory.Create(Constants.TranscriptionAiModelMap[SelectedProfile.TranscriptionModelId]);
            _chatAiService = _chatFactory.Create(Constants.ChatAiModelMap[SelectedProfile.ChatModelId]);

            // transcriptTextをワイプしてない場合はTranscribeをスキップする。API費用節約と高速化のため。
            if (_transcriptText == null)
            {
                if (_waveFilePath == null) throw new InvalidOperationException();
                _transcriptText = await _transcriptionAiService.TranscribeAsync(_waveFilePath);
            }

            // DEBUG TEXT
            var chatResponseText = await _chatAiService.CompleteChatAsync(SelectedProfile.SystemPrompt, _transcriptText, ct);

            ProcessedText = chatResponseText.Value;

            _clipboard.Paste(ProcessedText);
        }
        catch (Exception ex) when (ex is OperationCanceledException || ex is TaskCanceledException)
        {
            return;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Noto note");
        }
        _machine.Fire(Trigger.CompletedProcessing);
    }

    private void OnExitProcessing()
    {
        _processingCtx?.Cancel();
        _processingCtx?.Dispose();
    }

    private void OnExitSettings()
    {
        Profiles = _profiles.GetAll();
        var activeId = _profiles.GetActiveProfileId();
        SelectedProfile = Profiles.FirstOrDefault(x => x.Id == activeId) ?? Profiles[0];

        OnPropertyChanged(nameof(Profiles));
    }

    private void SetStateFlags(State state)
    {
        IsIdle = state == State.Idle;
        IsRecording = state == State.Recording;
        IsProcessing = state == State.Processing;
        IsSettings = state == State.Settings;
    }

    private void HandleActivationHotkey()
    {
        switch (_machine.State)
        {
            case State.Idle:
                _window.SetTopmost(true);
                Task.Run(() => _machine.FireAsync(Trigger.StartRecording));
                break;
            case State.Recording:
                _window.SetTopmost(false);
                Task.Run(() => _machine.FireAsync(Trigger.StopRecording));
                break;
            default:
                // nothing to do
                break;
        }
    }

    private void HandleCancelHotkey()
    {
        switch (_machine.State)
        {
            case State.Recording:
                _window.Activate();
                Task.Run(() => _machine.FireAsync(Trigger.Cancel));
                break;
            case State.Processing:
                _window.Activate();
                _processingCtx.Cancel();
                Task.Run(() => _machine.FireAsync(Trigger.Cancel));
                break;
            default:
                // nothing to do
                break;
        }
    }

    private void HandleProfileToggleHotkey()
    {
        switch (_machine.State)
        {
            case State.Idle:
                _window.Activate();
                var profiles = Profiles.ToList();
                var index = profiles.IndexOf(SelectedProfile);
                var profilesCount = profiles.Count;
                SelectedProfile = profiles[(index + 1) % profilesCount];
                break;
            default:
                break;
        }
    }

    [RelayCommand]
    public void StartRecording() => _machine.Fire(Trigger.StartRecording);

    [RelayCommand]
    public void Cancel() => _machine.Fire(Trigger.Cancel);

    [RelayCommand]
    public void Retry() => Task.Run(() => _machine.FireAsync(Trigger.Retry));

    [RelayCommand]
    public void EnterSettings() => _machine.Fire(Trigger.EnterSettings);

    [RelayCommand]
    public void ExitSettings() => _machine.Fire(Trigger.ExitSettings);


}