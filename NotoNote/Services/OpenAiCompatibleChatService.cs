using NotoNote.Models;
using OpenAI;
using OpenAI.Chat;

namespace NotoNote.Services;

public sealed class OpenAiCompatibleChatService : IChatService
{
    private readonly ChatClient _client;

    public OpenAiCompatibleChatService(ApiKey apiKey, Uri endpoint, ApiModelId model)
    {
        _client = new OpenAIClient(new(apiKey.Value), new OpenAIClientOptions()
        {
            Endpoint = endpoint,
        }).GetChatClient(model.Value);
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
