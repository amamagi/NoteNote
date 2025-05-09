using LiteDB;
using NotoNote.Models;
using NotoNote.Services;
using System.IO;

namespace NotoNote.DataStore;
public sealed class LiteDbProfileRepository : IProfileRepository, IDisposable
{
    private static readonly string DefaultPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "NotoNote",
        "data.db");

    private readonly LiteDatabase _db;

    public LiteDbProfileRepository(IPresetProfileProvider presetProfileProvider) : this(presetProfileProvider, DefaultPath) { }

    public LiteDbProfileRepository(IPresetProfileProvider presetProfileProvider, string dbPath)
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

        // Seed if empty
        if (_collection.Count() == 0)
        {
            foreach (var profile in presetProfileProvider.Get())
            {
                _collection.Insert(profile.ToDto());
            }
        }
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