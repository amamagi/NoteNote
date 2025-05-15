using LiteDB;
using NotoNote.Models;
using NotoNote.Services;

namespace NotoNote.DataStore;
public sealed class ProfileRepository : IProfileRepository
{
    private readonly ILiteCollection<ProfileDto> _profiles;
    private readonly ILiteCollection<GuidDto> _guids;
    public ProfileRepository(IPresetProfileProvider presetProfileProvider, ILiteDbContext dbContext)
    {
        _profiles = dbContext.Profiles;
        _guids = dbContext.Guids;

        // Seed if empty
        if (_profiles.Count() == 0)
        {
            var presets = presetProfileProvider.Get();
            if (presets.Count() == 0)
            {
                presets.Add(Profile.Default);
            }

            var tail = new GuidDto("profiles_tail", Guid.NewGuid());

            var tailIndex = presets.Count();

            for (var i = 0; i <= tailIndex; i++)
            {
                if (i == 0)
                {

                }
                if (i == tailIndex)
                {
                    _profiles.Insert(presets[i].ToDto(tail.Guid));
                }
            }
        }
    }
    public void Add(Profile profile)
    {
        _profiles.Insert(profile.ToDto(_profiles.Count()));
    }
    public Profile? Get(ProfileId id)
    {
        var dto = _profiles.FindById(id.Value);
        return dto?.ToModel();
    }

    public IEnumerable<Profile> GetAll()
    {
        return _profiles.FindAll()
            .OrderBy(x => x.Order)
            .Select(x => x.ToModel());
    }

    public void Update(Profile profile)
    {
        _profiles.Update(profile.ToDto());
    }
    public void Delete(ProfileId id)
    {
        _profiles.Delete(id.Value);
    }

    private
}