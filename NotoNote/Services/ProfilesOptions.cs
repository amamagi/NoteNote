using System.Collections.Generic;

namespace NotoNote.Services;

public sealed class PresetProfileOptions
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public IEnumerable<string> SystemPrompts { get; set; } = [];
}