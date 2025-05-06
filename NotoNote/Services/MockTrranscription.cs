using NotoNote.Models;

namespace NotoNote.Services;
public sealed class MockTrranscription : ITranscriptionAiService
{
    public Task<TranscriptText> TranscribeAsync(WaveFilePath filePath)
    {
        return Task.FromResult<TranscriptText>(new("（モック）こんにちは、録音テスト中です。"));
    }
}
