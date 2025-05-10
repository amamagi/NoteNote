using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using Stateless;
using System;

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

    private readonly IHotKeyService _hotKey;
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

    /// <summary>
    /// Idle画面でProcessedTextを表示可能か
    /// </summary>
    public bool ShowProcessedText => IsIdle && !string.IsNullOrEmpty(ProcessedText);

    public MainScreenViewModel(
        IHotKeyService hotKey,
        IProfileRepository profiles,
        IAudioService audio,
        ITranscriptionAiServiceFactory transcription,
        IChatAiServiceFactory chat)
    {
        _hotKey = hotKey;
        _profiles = profiles;
        _audio = audio;
        _transcription = transcription;
        _chat = chat;

        _selectedProfile = _profiles.GetAll().First();

        _machine = new(State.Idle);
        ConfigureStateMachine();
        SetStateFlags(_machine.State);
        _machine.OnTransitioned(t => SetStateFlags(t.Destination));
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

    [RelayCommand]
    public async Task HandleHotkeyAsync()
    {
        var current = _machine.State;

        switch (current)
        {
            case State.Idle:
                _machine.Fire(Trigger.StartRecording);
                break;
            case State.Recording:
                _machine.Fire(Trigger.StopRecording);
                await ProcessTranscriptionAsync();
                break;
        }
    }

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