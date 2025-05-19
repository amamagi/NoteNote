using NotoNote.Models;

namespace NotoNote.Services;

public sealed class ChatServiceFactory : IChatServiceFactory
{
    private readonly IApiKeyRepository _apiKeys;
    private ApiKey OpenAiApiKey => _apiKeys.Get(ApiProvider.OpenAI) ?? throw new ArgumentException("OpenAI API Key not found");

    public ChatServiceFactory(IApiKeyRepository apiKeys) => _apiKeys = apiKeys;
    public IChatService Create(IChatModel model)
    {
        return model switch
        {
            OpenAiChatAiModel om => new OpenAiChatService(om.ApiId, OpenAiApiKey),
            _ => throw new ArgumentOutOfRangeException(nameof(model.GetType)),
        };
    }
}
