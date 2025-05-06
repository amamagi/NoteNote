using NotoNote.Models;

namespace NotoNote.Services;

public sealed class ChatAiServiceFactory : IChatAiServiceFactory
{
    private readonly IApiKeyRegistry _apiKeys;

    public ChatAiServiceFactory(IApiKeyRegistry apiKeyRegistry) => _apiKeys = apiKeyRegistry;
    public IChatAiService Create(IChatAiModel model)
    {
        return model switch
        {
            OpenAiChatAiModel om => new OpenAiChatService(om.ApiId, _apiKeys.Keys[AiProvider.OpenAI]),
            _ => throw new ArgumentOutOfRangeException(nameof(model.GetType)),
        };
    }
}
