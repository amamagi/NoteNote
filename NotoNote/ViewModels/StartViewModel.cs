using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using NotoNote.Services;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using Application = System.Windows.Application;

namespace NotoNote.ViewModels;
public partial class StartViewModel : ObservableObject, IDisposable
{
    private readonly IHotKeyService _hotKeyService;
    private readonly IProfileRegistry _profiles;
    private readonly IAudioService _audioService;
    private readonly ITranscriptionAiServiceFactory _transcription;
    private readonly IChatAiServiceFactory _chat;

    public IEnumerable<Profile> AvailableProfiles => _profiles.Profiles;

    [ObservableProperty] private Profile _selectedProfile;
    [ObservableProperty] private string? _profileName;
    [ObservableProperty] private string _status = "Idle";
    [ObservableProperty] private string _resultText = "";

    public StartViewModel(
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

        _selectedProfile = profiles.Profiles.FirstOrDefault() ?? throw new ArgumentException();

        _hotKeyService.SetCallback(() => ToggleRecordingCommand.Execute(null));
    }


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
        catch (Exception ex)
        {
            Status = "Error";
            MessageBox.Show(ex.Message, "noto note");
        }
    }

    public void Dispose()
    {
        _hotKeyService.Clean();
    }
}
