namespace NotoNote.Models;
public interface ITranscriptionAiService
{
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    Task<TranscriptText> TranscribeAsync(WaveFilePath filePath, CancellationToken ct = default);
}
