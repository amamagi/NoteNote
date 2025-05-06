using NotoNote.Models;

namespace NotoNote.Services;

public sealed class TranscriptionAiServiceFactory : ITranscriptionAiServiceFactory
{
    private readonly IApiKeyRegistry _apiKeys;

    public TranscriptionAiServiceFactory(IApiKeyRegistry apiKeyRegistry) => _apiKeys = apiKeyRegistry;

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ITranscriptionAiService Create(ITranscriptionAiModel model)
    {
        return model switch
        {
            OpenAiTranscribeAiModel om => new OpenAiTranscriptionService(om.ApiId, _apiKeys.Keys[AiProvider.OpenAI]),
            _ => throw new ArgumentOutOfRangeException(nameof(model.GetType)),
        };
    }
}
