
using NotoNote.Utilities;

namespace NotoNote.Models;

public interface ITranscriptionModel
{
    public ModelName DisplayName { get; }
}

public interface IChatModel
{
    public ModelName DisplayName { get; }
}
public record ModelName(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}
