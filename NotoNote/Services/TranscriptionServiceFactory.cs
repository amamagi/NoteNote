using NotoNote.Models;

namespace NotoNote.Services;

public sealed class TranscriptionServiceFactory : ITranscriptionServiceFactory
{
    private readonly IApiKeyRepository _apiKeys;
    private ApiKey OpenAiApiKey => _apiKeys.Get(ApiProvider.OpenAI) ?? throw new ArgumentException("OpenAI API Key not found");

    public TranscriptionServiceFactory(IApiKeyRepository apiKeys) => _apiKeys = apiKeys;

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ITranscriptionService Create(ITranscriptionModel model)
    {
        return model switch
        {
            OpenAiTranscribeAiModel om => new OpenAiTranscriptionService(om.ApiId, OpenAiApiKey),
            _ => throw new ArgumentOutOfRangeException(nameof(model.GetType)),
        };
    }
}
