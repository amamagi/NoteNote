using Microsoft.Extensions.Options;
using NotoNote.Models;
using OpenAI.Chat;

namespace NotoNote.Services;

public sealed class OpenAiChatService : IChatAiService
{
    private readonly ChatClient _client;

    public OpenAiChatService(OpenAiApiId model, ApiKey apiKey)
    {
        _client = new ChatClient(model.Value, apiKey.Value);
    }

    public async Task<ChatResponceText> CompleteChatAsync(SystemPrompt systemPrompt, TranscriptText transcript)

    {
        var completion = await _client.CompleteChatAsync(systemPrompt.Value, transcript.Value);
        var response = completion.Value.Content[0].Text;
        return new(response);
    }

}
