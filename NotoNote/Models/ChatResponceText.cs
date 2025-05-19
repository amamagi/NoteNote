using NotoNote.Utilities;

namespace NotoNote.Models;

public record ChatResponceText(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}
