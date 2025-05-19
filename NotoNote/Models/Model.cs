using NotoNote.Utilities;

namespace NotoNote.Models;

public interface ITranscriptionModel
{
    public TranscriptionModelId Id { get; }
    public ModelName DisplayName { get; }
}

public interface IChatModel
{
    public ChatModelId Id { get; }
    public ModelName DisplayName { get; }
}

public record TranscriptionModelId(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}
public record ChatModelId(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}

public record ModelName(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}


