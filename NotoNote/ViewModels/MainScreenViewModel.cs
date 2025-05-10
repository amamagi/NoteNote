using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using Stateless;

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

    private readonly IWindowService _window;
    private readonly IHotkeyService _hotKey;
    private readonly IProfileRepository _profiles;
    private readonly IAudioService _audio;
    private readonly ITranscriptionAiServiceFactory _transcription;
    private readonly IChatAiServiceFactory _chat;
    private readonly StateMachine<State, Trigger> _machine;

    [ObservableProperty] private bool _isIdle;
    [ObservableProperty] private bool _isRecording;
    [ObservableProperty] private bool _isProcessing;
    [ObservableProperty] private bool _isSettings;
    [ObservableProperty] private Profile _selectedProfile;
    [ObservableProperty] private string _transcriptionText = "";
    [ObservableProperty] private string _processedText = "";
    [ObservableProperty] private string _activationHotkeyText = "";
    [ObservableProperty] private string _stopRecordingHotkeyText = "";

    public IEnumerable<Profile> Profiles => _profiles.GetAll();

    /// <summary>
    /// Idle画面でProcessedTextを表示可能か
    /// </summary>
    public bool ShowProcessedText => IsIdle && !string.IsNullOrEmpty(ProcessedText);


    public MainScreenViewModel(
        IHotkeyService hotKey,
        IProfileRepository profiles,
        IAudioService audio,
        ITranscriptionAiServiceFactory transcription,
        IChatAiServiceFactory chat,
        IWindowService window)
    {
        _hotKey = hotKey;
        _profiles = profiles;
        _audio = audio;
        _transcription = transcription;
        _chat = chat;
        _window = window;

        _selectedProfile = _profiles.GetAll().First();

        _machine = new(State.Idle);
        ConfigureStateMachine();
        SetStateFlags(_machine.State);
        _machine.OnTransitioned(t =>
        {
            SetStateFlags(t.Destination);
        });

        _hotKey.RegisterHotkey(ActivationHotkey, HandleActivationHotkey);
        _hotKey.RegisterHotkey(CancelHotkey, HandleCancelHotkey);

        ActivationHotkeyText = GetHotkeyText(ActivationHotkey) + " : Start recording";
        StopRecordingHotkeyText = GetHotkeyText(ActivationHotkey) + " : Stop recording\nESC: Cancel";

    }

    private string GetHotkeyText(Hotkey hotkey)
    {
        return hotkey.Modifiers.ToString().Replace(", ", "+") + "+" + hotkey.Key.ToString();
    }

    private void ConfigureStateMachine()
    {
        _machine.Configure(State.Idle)
            .OnEntry(() => OnPropertyChanged(nameof(ShowProcessedText)))
            .Permit(Trigger.StartRecording, State.Recording)
            .Permit(Trigger.EnterSettings, State.Settings)
            // 処理済みテキストがあるなら再処理が可能
            .PermitIf(Trigger.Retry, State.Processing, () => !string.IsNullOrEmpty(ProcessedText));

        _machine.Configure(State.Recording)
            .Permit(Trigger.StopRecording, State.Processing)
            .Permit(Trigger.Cancel, State.Idle);

        _machine.Configure(State.Processing)
            .Permit(Trigger.CompletedProcessing, State.Idle)
            .Permit(Trigger.Cancel, State.Idle);

        _machine.Configure(State.Settings)
            .Permit(Trigger.ExitSettings, State.Idle);
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
                _window.Activate();
                _machine.Fire(Trigger.StartRecording);
                break;
            case State.Recording:
                _window.Activate();
                _machine.Fire(Trigger.StopRecording);
                Task.Run(() => ProcessTranscriptionAsync());
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

    private async Task ProcessTranscriptionAsync()
    {
        await Task.Delay(1000);
        ProcessedText = "Dummy";
        _machine.Fire(Trigger.CompletedProcessing);
    }

}