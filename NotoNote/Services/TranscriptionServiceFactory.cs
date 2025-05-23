using NotoNote.Models;

namespace NotoNote.Services;

public sealed class TranscriptionServiceFactory(
    IApiKeyRepository _apiKeyRepository,
    IApiMetadataProvider _apiMetadataRepository) : ITranscriptionServiceFactory
{
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ITranscriptionService Create(ITranscriptionModel model)
    {
        if (model is OpenAiCompatibleTranscriptionModel om)
        {
            var metadata = _apiMetadataRepository.Get(om.ApiSource) ?? throw new ArgumentException($"{om.ApiSource} API metadata not found");
            var apiKey = _apiKeyRepository.Get(om.ApiSource) ?? throw new ArgumentException($"{om.ApiSource} API Key not found");
            return new OpenAiCompatibleTranscriptionService(apiKey, metadata.BaseUri, om.ApiId);
            // TODO: serviceはキャッシュした方がいいかも
        }

        throw new NotImplementedException($"Transcription model {model.GetType()} is not implemented.");
    }
}
