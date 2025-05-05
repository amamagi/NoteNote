using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Services;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace NotoNote.ViewModels;
public partial class MainViewModel : ObservableObject
{
    private readonly IAudioService _audioService;

    public MainViewModel() : this(new AudioService()) { }
    public MainViewModel(IAudioService audioService) => _audioService = audioService;

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

                    byte[] wav = await _audioService.StopRecordingAsync();

                    Task.Run(async () => await File.WriteAllBytesAsync("last.wav", wav));

                    string transcript = await MockWhisperAsync(wav);

                    ResultText = transcript;
                    Clipboard.SetText(ResultText);
                    Status = "Idle";
                    break;
            }
        }catch(Exception e)
        {
            Status = "Error";
            MessageBox.Show(e.Message, "noto note");
        }
    }

    private static Task<string> MockWhisperAsync(byte[] _)
    {
        return Task.FromResult<string>("[Mock] LLM整形済みテキスト");
    }
}
