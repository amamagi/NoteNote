using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using System.Diagnostics;

namespace NotoNote.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{



    public MainWindowViewModel(
        IHotKeyService hotKeyService,
    IProfileRegistry profiles,
    IAudioService audioService,
    ITranscriptionAiServiceFactory transcription,
    IChatAiServiceFactory chat)
    {


        _hotKeyService = hotKeyService;
        _profiles = profiles;
        _audioService = audioService;
        _transcription = transcription;
        _chat = chat;

        TransitionViewCommand.Execute("Start");
    }

    private readonly IHotKeyService _hotKeyService;
    private readonly IProfileRegistry _profiles;
    private readonly IAudioService _audioService;
    private readonly ITranscriptionAiServiceFactory _transcription;
    private readonly IChatAiServiceFactory _chat;




    [ObservableProperty]
    private ObservableObject _activeView = new EmptyViewModel();

    [RelayCommand]
    private void TransitionView(string param)
    {
        Debug.WriteLine($"TransitionView: {param}"); // Debugging line
        switch (param)
        {
            case "Start":
                ActiveView = new StartViewModel(_hotKeyService, _profiles, _audioService, _transcription, _chat);
                break;
            case "Settings":
                if (ActiveView is IDisposable d) d.Dispose();
                ActiveView = new SettingsViewModel(_profiles);
                break;
            default:
                ActiveView = new EmptyViewModel();
                break;
        }
    }

}
