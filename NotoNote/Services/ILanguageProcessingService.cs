namespace NotoNote.Services;
public interface ILanguageProcessingService
{
    Task<string> ProcessTranscriptAsync(string transcript);
}
