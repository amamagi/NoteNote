namespace NotoNote.Services;

sealed class MockLanguageProcessor : ILanguageProcessingService
{
    public Task<string> ProcessTranscriptAsync(string transcript)
    {
        return Task.FromResult<string>(transcript);
    }
}
