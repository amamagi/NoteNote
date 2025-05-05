namespace NotoNote.Services;
public interface IAudioService : IDisposable
{
    void StartRecording();
    Task<string> StopRecordingAsync();
    }
