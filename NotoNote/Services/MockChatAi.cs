using NotoNote.Models;

namespace NotoNote.Services;

sealed class MockChatAi : IChatAiService
{
    public Task<ChatResponceText> CompleteChatAsync(SystemPrompt systemPrompt, TranscriptText transcript, CancellationToken ct = default)
    {
        return Task.FromResult<ChatResponceText>(new(transcript.Value));
    }
}
