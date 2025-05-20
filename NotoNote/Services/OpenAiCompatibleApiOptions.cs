namespace NotoNote.Services;

public sealed class OpenAiCompatibleApiOptions
{
    public string Name { get; init; } = "";
    public string ApiKey { get; init; } = "";
    public string BaseUrl { get; init; } = "";
    public List<OpenAiCompatibleModelOptions> TranscriptionModels { get; init; } = new();
    public List<OpenAiCompatibleModelOptions> ChatModels { get; init; } = new();
}

public sealed class OpenAiCompatibleModelOptions
{
    public string Name { get; init; } = "";
    public string ApiId { get; init; } = "";
}
