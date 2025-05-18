using LiteDB;
using NotoNote.Models;

namespace NotoNote.DataStore;
public sealed class HotkeyDto
{
    [BsonId]
    public int Id { get; set; }
    public Keys Key { get; set; }
    public Keys Modifiers { get; set; }
}

public static class HotkeyDtoExtensions
{
    public static Hotkey ToModel(this HotkeyDto dto)
    {
        return new Hotkey(dto.Key, dto.Modifiers);
    }
    public static HotkeyDto ToDto(this Hotkey hotkey, HotkeyPurpose purpose)
    {
        return new HotkeyDto()
        {
            Id = (int)purpose,
            Key = hotkey.Key,
            Modifiers = hotkey.Modifiers
        };
    }
}
