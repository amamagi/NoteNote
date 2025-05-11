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

    public async Task<ChatResponceText> CompleteChatAsync(SystemPrompt systemPrompt, TranscriptText transcript, CancellationToken ct = default)
    {
        var systemMessage = new SystemChatMessage(systemPrompt.Value);
        var userMessage = new UserChatMessage(transcript.Value);
        var completion = await _client.CompleteChatAsync([systemMessage, userMessage], cancellationToken: ct);
        var response = completion.Value.Content[0].Text;
        return new(response);
    }
}
