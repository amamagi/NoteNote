namespace NotoNote.Models;

public record  RecordWithValidation
{
    protected RecordWithValidation()
    {
        Validate();
    }

    protected virtual void Validate()
    {
    }
}
