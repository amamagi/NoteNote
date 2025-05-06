namespace NotoNote.Models;
public interface IAudioService : IDisposable
{
    void StartRecording();
    Task<string> StopRecordingAsync();
    }
