using NotoNote.Utilities;

namespace NotoNote.Models;

public record TranscriptText(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}
