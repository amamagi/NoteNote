namespace NotoNote.Models;
public interface IAudioService : IDisposable
{
    void StartRecording(Action? timeoutCallback);
    Task<WaveFilePath?> StopRecordingAsync();
}
