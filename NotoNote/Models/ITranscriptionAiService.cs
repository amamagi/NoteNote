namespace NotoNote.Models;
public interface ITranscriptionAiService
{
    Task<TranscriptText> TranscribeAsync(WaveFilePath filePath);
}
