using NotoNote.Models;

namespace NotoNote.Services;

public sealed class TranscriptionAiServiceLocator : ITranscriptionAiServiceLocator
{
    private readonly IApiKeyRegistry _apiKeys;

    public TranscriptionAiServiceLocator(IApiKeyRegistry apiKeyRegistry) => _apiKeys = apiKeyRegistry;

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ITranscriptionAiService GetService(ITranscriptionAiModel model)
    {
        return model switch
        {
            OpenAiTranscribeAiModel om => new OpenAiTranscriptionService(om.ApiId, _apiKeys.Keys[AiProvider.OpenAI]),
            _ => throw new ArgumentOutOfRangeException(nameof(model.GetType)),
        };
    }
}
