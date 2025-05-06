namespace NotoNote.Services;

sealed class MockChatAi : IChatAiService
{
    public Task<string> CompleteChatAsync(string systemPrompt, string transcript)
    {
        return Task.FromResult<string>(transcript);
    }
}
