namespace NotoNote.Models;
public interface ITranscriptionAiService
{
    Task<string> TranscribeAsync(string audioFilePath);
}
