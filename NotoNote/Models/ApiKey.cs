namespace NotoNote.Models;

public record ApiKey(ApiSource Source, string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(Value)) throw new ArgumentException("ApiKey.Value must not be null or empty");
    }
}