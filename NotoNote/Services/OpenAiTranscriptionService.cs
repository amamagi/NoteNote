using NotoNote.Models;
using OpenAI.Audio;
using System.Diagnostics;

namespace NotoNote.Services;

public sealed class OpenAiTranscriptionService : ITranscriptionAiService
{
    private readonly AudioClient _client;

    public OpenAiTranscriptionService(OpenAiApiId model, ApiKey apiKey)
    {
        _client = new(model.Value, apiKey.Value);
    }

    public async Task<TranscriptText> TranscribeAsync(WaveFilePath filePath)
    {
        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Simple
        };

        var response = await _client.TranscribeAudioAsync(filePath.Value, options);

        Debug.WriteLine($"{response.Value.Text}");

        return new(response.Value.Text);
    }
}
