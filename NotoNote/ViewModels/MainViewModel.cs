using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using NotoNote.Services;
using MessageBox = System.Windows.MessageBox;

namespace NotoNote.ViewModels;
public partial class MainViewModel(
    IProfileRegistry profiles,
    IAudioService audioService,
    ITranscriptionAiServiceLocator transcription,
    IChatAiServiceLocator chat) : ObservableObject
{
    private readonly IProfileRegistry _profiles = profiles;
    private readonly IAudioService _audioService = audioService;
    private readonly ITranscriptionAiServiceLocator _transcription = transcription;
    private readonly IChatAiServiceLocator _chat = chat;

    [ObservableProperty] private string _profile = "";
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

                    var profile = _profiles.Profiles.FirstOrDefault() ?? throw new ArgumentException();

                    // 1. record
                    var audioFilePath = await _audioService.StopRecordingAsync();

                    // 2. transcribe
                    var transcriptionService = _transcription.GetService(profile.TranscriptionModel);
                    var transcript = await transcriptionService.TranscribeAsync(audioFilePath);

                    // 4. chat
                    var chatService = _chat.GetService(profile.ChatModel);
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
