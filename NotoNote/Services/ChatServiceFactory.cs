using NotoNote.Models;

namespace NotoNote.Services;

public sealed class ChatServiceFactory(
    IApiKeyRepository _apiKeyRepository,
    IApiMetadataProvider _apiMetadataRepository) : IChatServiceFactory
{
    public IChatService Create(IChatModel model)
    {
        if (model is OpenAiCompatibleChatModel om)
        {
            var apiMetadata = _apiMetadataRepository.Get(om.ApiSource) ?? throw new ArgumentException($"{om.ApiSource} API Metadata not found");
            var apiKey = _apiKeyRepository.Get(om.ApiSource) ?? throw new ArgumentException($"{om.ApiSource} API Key not found");
            return new OpenAiCompatibleChatService(apiKey, apiMetadata.BaseUri, om.ApiId);
            // TODO: serviceはキャッシュした方がいいかも
        }

        throw new NotImplementedException($"Chat model {model.GetType()} is not implemented.");
    }
}
