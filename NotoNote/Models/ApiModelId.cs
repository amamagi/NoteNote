using NotoNote.Utilities;

namespace NotoNote.Models;
public record ApiModelId(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}
