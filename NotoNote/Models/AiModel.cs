namespace NotoNote.Models;

public interface ITranscriptionAiModel
{
    public TranscriptionAiModelId Id { get; }
    public AiModelName DisplayName { get; }
}

public interface IChatAiModel
{
    public ChatAiModelId Id { get; }
    public AiModelName DisplayName { get; }
}
public record TranscriptionAiModelId(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}
public record ChatAiModelId(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}



