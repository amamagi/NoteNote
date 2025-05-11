using NotoNote.Models;

namespace NotoNote.Services;

public sealed class ChatAiServiceFactory : IChatAiServiceFactory
{
    private readonly IApiKeyRepository _apiKeys;
    private ApiKey OpenAiApiKey => _apiKeys.Get(ApiProvider.OpenAI) ?? throw new ArgumentException("OpenAI API Key not found");

    public ChatAiServiceFactory(IApiKeyRepository apiKeys) => _apiKeys = apiKeys;
    public IChatAiService Create(IChatAiModel model)
    {
        return model switch
        {
            OpenAiChatAiModel om => new OpenAiChatService(om.ApiId, OpenAiApiKey),
            _ => throw new ArgumentOutOfRangeException(nameof(model.GetType)),
        };
    }
}
