using LiteDB;
using NotoNote.Models;
using NotoNote.Services;

namespace NotoNote.DataStore;
public sealed class ProfileRepository : IProfileRepository
{
    public ProfileRepository(IPresetProfileProvider presetProfileProvider, ILiteDbContext dbContext)
    {
        _collection = dbContext.Profiles;

        // Ensure indexes
        _collection.EnsureIndex(x => x.Id);

        // Seed if empty
        if (_collection.Count() == 0)
        {
            var counter = 0;
            foreach (var profile in presetProfileProvider.Get())
            {
                _collection.Insert(profile.ToDto(counter++));
            }
        }
    }

    private readonly ILiteCollection<ProfileDto> _collection;
    public void Add(Profile profile)
    {
        _collection.Insert(profile.ToDto(_collection.Count()));
    }
    public Profile? Get(ProfileId id)
    {
        var dto = _collection.FindById(id.Value);
        return dto?.ToModel();
    }

    public IEnumerable<Profile> GetAll()
    {
        return _collection.FindAll()
            .OrderBy(x => x.Order)
            .Select(x => x.ToModel());
    }

    public void Update(Profile profile)
    {
        _collection.Update(profile.ToDto());
    }
    public void Delete(ProfileId id)
    {
        _collection.Delete(id.Value);
    }
}