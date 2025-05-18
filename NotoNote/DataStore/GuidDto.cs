using LiteDB;

namespace NotoNote.DataStore;
public sealed class GuidDto
{
    public GuidDto(string key, Guid value)
    {
        Key = key;
        Value = value;
    }

    [BsonId]
    public string Key { get; set; } = "";
    public Guid Value { get; set; }
}