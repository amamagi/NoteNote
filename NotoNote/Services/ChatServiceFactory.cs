using NotoNote.Models;

namespace NotoNote.Services;

public sealed class ChatServiceFactory : IChatServiceFactory
{
    private readonly IApiKeyRepository _apiKeys;
    public ChatServiceFactory(IApiKeyRepository apiKeys) => _apiKeys = apiKeys;
    public IChatService Create(IChatModel model)
    {
        if (model is OpenAiCompatibleChatModel om)
        {
            var apiKey = _apiKeys.Get(om.ApiSource.ApiSource) ?? throw new ArgumentException($"{om.ApiSource} API Key not found");
            return new OpenAiCompatibleChatService(apiKey, om.ApiSource.Uri, om.ApiId);
            // TODO: serviceはキャッシュした方がいいかも
        }

        throw new NotImplementedException($"Chat model {model.GetType()} is not implemented.");
    }
}
