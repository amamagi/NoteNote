using Microsoft.Extensions.Options;
using NotoNote.Models;

namespace NotoNote.Services;

public sealed class OptionApiKeyRegistry : IApiKeyRegistry
{
    public OptionApiKeyRegistry(IOptions<OpenAiOptions> openAiOptions)
    {
        try
        {
            Keys.Add(AiProvider.OpenAI, new(openAiOptions.Value.ApiKey));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "NotoNote");
        }
    }

    public Dictionary<AiProvider, ApiKey> Keys { get; } = new();
}
