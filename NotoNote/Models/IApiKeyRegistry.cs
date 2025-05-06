namespace NotoNote.Models;

public interface IApiKeyRegistry
{
    public Dictionary<AiProvider, ApiKey> Keys { get; }
}

public enum AiProvider
{
    OpenAI,

}
public record ApiKey(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(Value)) throw new ArgumentException();
    }
}
