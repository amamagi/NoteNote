namespace NotoNote.Services;
public interface ITranscriptionService
{
    Task<string> TranscribeAsync(byte[] wav);
}
