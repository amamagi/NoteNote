using LiteDB;

namespace NotoNote.DataStore;
public sealed class GuidDto
{
    public GuidDto(string id, Guid guid)
    {
        Id = id;
        Guid = guid;
    }

    [BsonId]
    public string Id { get; set; } = "";
    public Guid Guid { get; set; }
}