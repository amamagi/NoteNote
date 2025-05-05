
namespace NotoNote.Services;
public sealed class MockTrranscription : ITranscriptionService
{
    public Task<string> TranscribeAsync(string audioFilePath)
    {
        return Task.FromResult<string>("（モック）こんにちは、録音テスト中です。");
    }
}
