using System.IO;

namespace NotoNote.Services;
public interface ITranscriptionService
{
    Task<string> TranscribeAsync(string audioFilePath);
}
