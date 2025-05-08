using LiteDB;
using NotoNote.Models;
using System.IO;

namespace NotoNote.DataStore;
public sealed class LiteDbProfileRepository : IDisposable
{
    private static readonly string DefaultPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "NotoNote",
        "data.db");

    private readonly LiteDatabase _db;

    public LiteDbProfileRepository() : this(DefaultPath) { }

    public LiteDbProfileRepository(string dbPath)
    {
        // Ensure the directory exists
        var directory = Path.GetDirectoryName(dbPath);
        if (directory != null && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        _db = new LiteDatabase(dbPath);

        // Create the collection if it doesn't exist
        _collection = _db.GetCollection<ProfileDto>("profiles");

        // Ensure indexes
        _collection.EnsureIndex(x => x.Id);
    }

    private readonly ILiteCollection<ProfileDto> _collection;
    public void Add(Profile profile)
    {
        _collection.Insert(profile.ToDto());
    }
    public Profile? Get(ProfileId id)
    {
        var dto = _collection.FindById(id.Value);
        return dto?.ToModel();
    }

    public IEnumerable<Profile> GetAll()
    {
        return _collection.FindAll().Select(x => x.ToModel());
    }

    public void Update(Profile profile)
    {
        _collection.Update(profile.ToDto());
    }
    public void Delete(ProfileId id)
    {
        _collection.Delete(id.Value);
    }

    public void Dispose()
    {
        _db?.Dispose();
    }
}