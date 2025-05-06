namespace NotoNote.Models;

public interface ITranscriptionAiModel
{
    public TranscriptionAiModelId Id { get; }
    public string DisplayName { get; }
}

public interface IChatAiModel
{
    public ChatAiModelId Id { get; }
    public string DisplayName { get; }
}
public record TranscriptionAiModelId(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(Value)) throw new ArgumentException();
    }
}
public record ChatAiModelId(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(Value)) throw new ArgumentException();
    }
}



