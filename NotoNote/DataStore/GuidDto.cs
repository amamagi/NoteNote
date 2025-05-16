using LiteDB;

namespace NotoNote.DataStore;
public sealed class GuidDto
{
    public GuidDto(string key, Guid guid)
    {
        Key = key;
        Guid = guid;
    }

    [BsonId]
    public string Key { get; set; } = "";
    public Guid Guid { get; set; }
}