using Microsoft.Extensions.Options;
using NotoNote.Models;
using OpenAI.Chat;

namespace NotoNote.Services;

public sealed class OpenAiChatService : IChatAiService
{
    private readonly ChatClient _client;

    public OpenAiChatService(OpenAiChatAiModel model, ApiKey apiKey)
    {
        _client = new ChatClient(model.ApiId, apiKey.Value);
    }

    public async Task<string> CompleteChatAsync(string systemPrompt, string transcript)
    {
        var completion = await _client.CompleteChatAsync(systemPrompt, transcript);
        return completion.Value.Content[0].Text;
    }
}
