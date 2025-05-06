using NotoNote.Models;

namespace NotoNote.Services;
public sealed class MockTrranscription : ITranscriptionAiService
{
    public Task<string> TranscribeAsync(string audioFilePath)
    {
        return Task.FromResult<string>("（モック）こんにちは、録音テスト中です。");
    }
}
