namespace NotoNote.Models;
public interface IAudioService : IDisposable
{
    void StartRecording();
    Task<WaveFilePath> StopRecordingAsync();
}
