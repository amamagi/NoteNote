using System.Collections.Generic;

namespace NotoNote.Services;

public sealed class ProfileOptions
{
    public string Name { get; set; } = "";
    public IEnumerable<string> SystemPrompts { get; set; } = [];
}