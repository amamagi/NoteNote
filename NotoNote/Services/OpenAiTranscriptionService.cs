using NotoNote.Models;
using OpenAI.Audio;
using System.Diagnostics;
using System.IO;

namespace NotoNote.Services;

public sealed class OpenAiTranscriptionService : ITranscriptionAiService
{
    private readonly AudioClient _client;

    public OpenAiTranscriptionService(OpenAiApiId model, ApiKey apiKey)
    {
        _client = new(model.Value, apiKey.Value);
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
