using NotoNote.Models;

namespace NotoNote.Services;

public sealed class TranscriptionServiceFactory : ITranscriptionServiceFactory
{
    private readonly IApiKeyRepository _apiKeys;

    public TranscriptionServiceFactory(IApiKeyRepository apiKeys) => _apiKeys = apiKeys;

    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ITranscriptionService Create(ITranscriptionModel model)
    {
        if (model is OpenAiCompatibleTranscribeModel om)
        {
            var apiKey = _apiKeys.Get(om.ApiSource.ApiSource) ?? throw new ArgumentException($"{om.ApiSource} API Key not found");
            return new OpenAiCompatibleTranscriptionService(apiKey, om.ApiSource.Uri, om.ApiId);
            // TODO: serviceはキャッシュした方がいいかも
        }

        throw new NotImplementedException($"Transcription model {model.GetType()} is not implemented.");
    }
}
