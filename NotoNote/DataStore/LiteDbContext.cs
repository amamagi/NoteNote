using LiteDB;
using System.IO;

namespace NotoNote.DataStore;

public interface ILiteDbContext : IDisposable
{
    ILiteCollection<ProfileDto> Profiles { get; }
    ILiteCollection<DbMetadataDto> Metadata { get; }
    ILiteCollection<ApiKeyDto> ApiKeys { get; }
    ILiteCollection<GuidDto> Guids { get; }
}

public sealed class LiteDbContext : ILiteDbContext
{
    public ILiteCollection<ProfileDto> Profiles => _db.GetCollection<ProfileDto>("profiles");
    public ILiteCollection<DbMetadataDto> Metadata => _db.GetCollection<DbMetadataDto>("metadata");
    public ILiteCollection<ApiKeyDto> ApiKeys => _db.GetCollection<ApiKeyDto>("api_keys");
    public ILiteCollection<GuidDto> Guids => _db.GetCollection<GuidDto>("guids");

    private static readonly string DefaultPath = Path.Combine(
        Path.GetDirectoryName(Application.ExecutablePath)!,
        "data.db");

    private readonly LiteDatabase _db;

    public LiteDbContext() : this(DefaultPath) { }
    public LiteDbContext(string dbPath)
    {
        var directory = Path.GetDirectoryName(dbPath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        _db = new LiteDatabase(dbPath);
    }

    public void Dispose()
    {
        _db?.Dispose();
    }
}