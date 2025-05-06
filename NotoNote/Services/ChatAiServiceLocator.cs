using NotoNote.Models;

namespace NotoNote.Services;

public sealed class ChatAiServiceLocator : IChatAiServiceLocator
{
    private readonly IApiKeyRegistry _apiKeys;

    public ChatAiServiceLocator(IApiKeyRegistry apiKeyRegistry) => _apiKeys = apiKeyRegistry;
    public IChatAiService GetService(IChatAiModel model)
    {
        return model switch
        {
            OpenAiChatAiModel om => new OpenAiChatService(om, _apiKeys.Keys[AiProvider.OpenAI]),
            _ => throw new ArgumentOutOfRangeException(nameof(model.GetType)),
        };
    }
}
