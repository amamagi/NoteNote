using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    private readonly Hotkey ActivationHotkey = new(Keys.K, Keys.Shift | Keys.Control);
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

    private WaveFilePath? _waveFilePath;
    private ITranscriptionAiService? _transcriptionAiService;
    private IChatAiService? _chatAiService;
    private TranscriptText? _transcriptText;
    private ChatResponceText? _chatResponseText;

    public IEnumerable<Profile> Profiles => _profiles.GetAll();

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

        // TODO: DBからIDを提供して検索
        _selectedProfile = _profiles.GetAll().First();

        _machine = new(State.Idle);
        ConfigureStateMachine();

        _hotKey.RegisterHotkey(ActivationHotkey, HandleActivationHotkey);
        _hotKey.RegisterHotkey(CancelHotkey, HandleCancelHotkey);

        ActivationHotkeyText = GetHotkeyText(ActivationHotkey) + " : Start recording";
        StopRecordingHotkeyText = GetHotkeyText(ActivationHotkey) + " : Stop recording\nESC: Cancel";
        _clipboard = clipboard;
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
            // 処理済みテキストがあるなら再処理が可能
            .PermitIf(Trigger.Retry, State.Processing, () => HasProcessedText);

        _machine.Configure(State.Recording)
            .OnEntry(OnEntryRecording)
            .OnExitAsync(OnExitRecordingAsync)
            .Permit(Trigger.StopRecording, State.Processing)
            .Permit(Trigger.Cancel, State.Idle);

        _machine.Configure(State.Processing)
            .OnEntry(() => Task.Run(OnEntryProcessingAsync))
            .Permit(Trigger.CompletedProcessing, State.Idle)
            .Permit(Trigger.Cancel, State.Idle);

        _machine.Configure(State.Settings)
            .Permit(Trigger.ExitSettings, State.Idle);

        // _machine.Stateをフラグに反映
        SetStateFlags(_machine.State);
        _machine.OnTransitioned(t => SetStateFlags(t.Destination));
    }

    private void OnEntryIdle()
    {
        OnPropertyChanged(nameof(HasProcessedText));
    }

    private void OnEntryRecording()
    {
        //_audio.StartRecording();
        _waveFilePath = null;
        _transcriptText = null;
        _chatResponseText = null;
    }

    private async Task OnExitRecordingAsync()
    {
        //_waveFilePath = await _audio.StopRecordingAsync();
    }

    private async Task OnEntryProcessingAsync()
    {
        try
        {
            _transcriptionAiService = _transcriptionFactory.Create(Constants.TranscriptionAiModelMap[SelectedProfile.TranscriptionModelId]);
            _chatAiService = _chatFactory.Create(Constants.ChatAiModelMap[SelectedProfile.ChatModelId]);

            //if (_waveFilePath == null) throw new InvalidOperationException();

            // transcriptTextをワイプしてない場合はTranscribeをスキップする。節約のため。
            //if (_transcriptText == null)
            //{
            //    _transcriptText = await _transcriptionAiService.TranscribeAsync(_waveFilePath);
            //}

            _transcriptText = new("有権を争うカシミール地方で起きたテロ事件をめぐって、軍事行動の応酬が続いていたインドとパキスタンは、互いに攻撃を即時停止し、停戦することで合意しました。一方で、カシミール地方では両国の軍が厳重な警戒態勢を続けていて、このまま双方が攻撃を自制することができるのか予断を許さない情勢です。");

            if (_transcriptText == null) throw new InvalidOperationException();
            _chatResponseText = await _chatAiService.CompleteChatAsync(SelectedProfile.SystemPrompt, _transcriptText);

            if (_chatResponseText == null) ProcessedText = string.Empty;
            else ProcessedText = _chatResponseText.Value;

            _clipboard.Paste(ProcessedText);
        }
        finally
        {
            _machine.Fire(Trigger.CompletedProcessing);
        }
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
                _machine.Fire(Trigger.StartRecording);
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
            case State.Processing:
                _window.Activate();
                _machine.Fire(Trigger.Cancel);
                break;
            default:
                // nothing to do
                break;
        }
    }

    [RelayCommand]
    public void StartRecording() => _machine.Fire(Trigger.StartRecording);

    [RelayCommand]
    public void Cancel() => _machine.Fire(Trigger.Cancel);

    [RelayCommand]
    public void Retry() => _machine.Fire(Trigger.Retry);

    [RelayCommand]
    public void EnterSettings() => _machine.Fire(Trigger.EnterSettings);

    [RelayCommand]
    public void ExitSettings() => _machine.Fire(Trigger.ExitSettings);


}