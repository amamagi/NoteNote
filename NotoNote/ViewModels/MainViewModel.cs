using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Services;
using MessageBox = System.Windows.MessageBox;

namespace NotoNote.ViewModels;
public partial class MainViewModel(
    IAudioService audioService,
    ITranscriptionService transcriptionService,
    ILanguageProcessingService languageProcessingService) : ObservableObject
{
    private readonly IAudioService _audioService = audioService;
    private readonly ITranscriptionService _transcriptionService = transcriptionService;
    private readonly ILanguageProcessingService _languageProcessingService = languageProcessingService;

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

                    var audioFilePath = await _audioService.StopRecordingAsync();
                    var transcript = await _transcriptionService.TranscribeAsync(audioFilePath);
                    var processedTranscript = await _languageProcessingService.ProcessTranscriptAsync(transcript);

                    ResultText = processedTranscript;
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
