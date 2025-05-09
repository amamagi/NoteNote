using LiteDB;

namespace NotoNote.DataStore;
public sealed class DbMetadataDto
{
    [BsonId]
    public string Key { get; set; } = string.Empty;
    public int Version { get; set; } = 0;
}