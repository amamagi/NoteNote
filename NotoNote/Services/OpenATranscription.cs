using Microsoft.Extensions.Options;
using OpenAI.Audio;
using System.Diagnostics;

namespace NotoNote.Services;

public sealed class OpenATranscription : ITranscriptionService
{
    private readonly AudioClient _client;

    public OpenATranscription(IOptions<OpenAiOptions> options)
    {
        var model = options.Value.TranscriptionModel;
        var apiKey = options.Value.ApiKey;

        _client = new (model, apiKey);
    }

    public async Task<string> TranscribeAsync(string audioFilePath)
    {
        AudioTranscriptionOptions options = new()
        {
            ResponseFormat = AudioTranscriptionFormat.Simple
        };

        var response = await _client.TranscribeAudioAsync(audioFilePath, options);

        Debug.WriteLine($"{response.Value.Text}");

        return response.Value.Text;
    }
}
