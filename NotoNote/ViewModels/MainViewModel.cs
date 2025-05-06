using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using NotoNote.Services;
using MessageBox = System.Windows.MessageBox;

namespace NotoNote.ViewModels;
public partial class MainViewModel(
    IProfileRegistry profiles,
    IAudioService audioService,
    ITranscriptionAiServiceFactory transcription,
    IChatAiServiceFactory chat) : ObservableObject
{
    private readonly IProfileRegistry _profiles = profiles;
    private readonly IAudioService _audioService = audioService;
    private readonly ITranscriptionAiServiceFactory _transcription = transcription;
    private readonly IChatAiServiceFactory _chat = chat;
    
    public IEnumerable<Profile> AvailableProfiles => _profiles.Profiles;

    [ObservableProperty] private Profile _selectedProfile = profiles.Profiles.FirstOrDefault() ?? throw new ArgumentException();
    [ObservableProperty] private string? _profileName;
    [ObservableProperty] private string _status = "Idle";
    [ObservableProperty] private string _resultText = "";

    [RelayCommand]
    private async Task ToggleRecording()
    {
        try
        {
            switch (Status)
            {
                case "Idle":
                    _audioService.StartRecording();
                    Status = "Recording...";
                    break;
                case "Recording...":
                    Status = "Processing...";

                    var profile = SelectedProfile with { };

                    // 1. record
                    var audioFilePath = await _audioService.StopRecordingAsync();

                    // 2. transcribe
                    var transcriptionService = _transcription.Create(profile.TranscriptionModel);
                    var transcript = await transcriptionService.TranscribeAsync(audioFilePath);

                    // 4. chat
                    var chatService = _chat.Create(profile.ChatModel);
                    var chatResponse = await chatService.CompleteChatAsync(profile.SystemPrompt, transcript);

                    // 5. paste
                    ResultText = chatResponse.Value;
                    ClipBoardService.Paste(ResultText);
                    Status = "Idle";
                    break;
            }
        }
        catch (Exception e)
        {
            Status = "Error";
            MessageBox.Show(e.Message, "noto note");
        }
    }
}
