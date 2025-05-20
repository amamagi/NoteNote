using NotoNote.Models;
using OpenAI;
using OpenAI.Audio;
using System.Diagnostics;
using System.IO;

namespace NotoNote.Services;

public sealed class OpenAiCompatibleTranscriptionService : ITranscriptionService
{
    private readonly AudioClient _client;

    public OpenAiCompatibleTranscriptionService(ApiKey apiKey, Uri endpoint, ApiModelId model)
    {
        _client = new OpenAIClient(new(apiKey.Value), new()
        {
            Endpoint = endpoint,
        }).GetAudioClient(model.Value);
    }

    public async Task<TranscriptText> TranscribeAsync(WaveFilePath filePath, CancellationToken ct = default)
    {
        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Simple
        };
        var fileName = Path.GetFileName(filePath.Value);
        using FileStream audioStream = File.OpenRead(filePath.Value);
        var response = await _client.TranscribeAudioAsync(audioStream, fileName, options, ct);

        Debug.WriteLine($"{response.Value.Text}");

        return new(response.Value.Text);
    }
}
