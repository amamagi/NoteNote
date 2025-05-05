namespace NotoNote.Services;

public sealed class OpenAiOptions
{
    public string ApiKey { get; set; } = "";
    public string TranscriptionModel { get; set; } = "";
    public string LanguageModel { get; set; } = "";
}
