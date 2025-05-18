using LiteDB;
using NotoNote.Models;
using System.IO;

namespace NotoNote.DataStore;

public interface ILiteDbContext : IDisposable
{
    ILiteCollection<ProfileDto> Profiles { get; }
    ILiteCollection<DbMetadataDto> Metadata { get; }
    ILiteCollection<ApiKeyDto> ApiKeys { get; }
    ILiteCollection<GuidDto> Guids { get; }
    ILiteCollection<HotkeyDto> Hotkeys { get; }
}

public sealed class LiteDbContext : ILiteDbContext
{
    public ILiteCollection<ProfileDto> Profiles => _db.GetCollection<ProfileDto>("profiles");
    public ILiteCollection<DbMetadataDto> Metadata => _db.GetCollection<DbMetadataDto>("metadata");
    public ILiteCollection<ApiKeyDto> ApiKeys => _db.GetCollection<ApiKeyDto>("api_keys");
    public ILiteCollection<GuidDto> Guids => _db.GetCollection<GuidDto>("guids");
    public ILiteCollection<HotkeyDto> Hotkeys => _db.GetCollection<HotkeyDto>("hotkeys");

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



        Profiles.EnsureIndex(x => x.Id);
        Profiles.EnsureIndex(x => x.NextId);
        Guids.EnsureIndex(x => x.Key);
        Hotkeys.EnsureIndex(x => x.Id);
    }

    public void Dispose()
    {
        _db?.Dispose();
    }
}