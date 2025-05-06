using Microsoft.Extensions.Options;
using NotoNote.Models;
using OpenAI.Audio;
using System.Diagnostics;

namespace NotoNote.Services;

public sealed class OpenAiTranscriptionService : ITranscriptionAiService
{
    private readonly AudioClient _client;

    public OpenAiTranscriptionService(OpenAiTranscribeAiModel model, ApiKey apiKey)
    {
        _client = new(model.ApiId, apiKey.Value);
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
