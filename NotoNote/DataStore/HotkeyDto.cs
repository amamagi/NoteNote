using LiteDB;

namespace NotoNote.DataStore;
public sealed class HotkeyDto
{
    [BsonId]
    public string HotkeyId { get; set; } = "";
    public string Key { get; set; } = "";
    public string Shift { get; set; } = "";
    public bool Ctrl { get; set; }
    public bool Alt { get; set; }

}