using Microsoft.Extensions.Options;
using NotoNote.Models;

namespace NotoNote.Services;

public sealed class ApiKeyRegistry : IApiKeyRegistry
{
    public ApiKeyRegistry(IOptions<OpenAiOptions> openAiOptions)
    {
        Keys.Add(AiProvider.OpenAI, new(openAiOptions.Value.ApiKey));
    }

    public Dictionary<AiProvider, ApiKey> Keys { get; } = new();
}
