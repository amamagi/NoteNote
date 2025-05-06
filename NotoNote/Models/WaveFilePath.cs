using System.IO;

namespace NotoNote.Models;

public record WaveFilePath(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
        if (!File.Exists(Value))
            throw new ArgumentException("File not found", Value);
        if (!Value.EndsWith(".wav"))
            throw new ArgumentException("File must be a .wav file");
    }
}

