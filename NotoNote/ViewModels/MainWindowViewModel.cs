using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using System.Diagnostics;

namespace NotoNote.ViewModels;

public partial class MainWindowViewModel(
    IProfileRegistry profiles,
    IAudioService audioService,
    ITranscriptionAiServiceFactory transcription,
    IChatAiServiceFactory chat) : ObservableObject
{
    [ObservableProperty]
    private ObservableObject _activeView = new EmptyViewModel();

    [RelayCommand]
    private void TransitionView(string param)
    {
        Debug.WriteLine($"TransitionView: {param}"); // Debugging line
        switch (param)
        {
            case "Start":
                ActiveView = new StartViewModel(profiles, audioService, transcription, chat);
                break;
            case "Settings":
                if (ActiveView is IDisposable d) d.Dispose(); 
                ActiveView = new SettingsViewModel(profiles);
                break;
            default:
                ActiveView = new EmptyViewModel();
                break;
        }
    }

}
