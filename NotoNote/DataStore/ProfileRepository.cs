using LiteDB;
using NotoNote.Models;
using NotoNote.Services;

namespace NotoNote.DataStore;
public sealed class ProfileRepository : IProfileRepository
{
    private const string HEAD_KEY = "profiles_head";
    private const string TAIL_KEY = "profiles_tail";
    private const string ACTIVE_KEY = "profile_active";
    private const string INVALID_PROFILE_LINKING = "Invalid profile linking.";

    private readonly ILiteCollection<ProfileDto> _profiles;
    private readonly ILiteCollection<GuidDto> _guids;
    private Guid Head => _guids.FindById(HEAD_KEY).Value;
    private Guid Tail { get; }

    public ProfileRepository(IPresetProfileProvider presetProfileProvider, ILiteDbContext dbContext)
    {
        _profiles = dbContext.Profiles;
        _guids = dbContext.Guids;

        // Get or initialize tail id
        if (_guids.FindById(TAIL_KEY) == null)
        {
            _guids.Insert(new GuidDto(TAIL_KEY, Guid.NewGuid()));
        }
        Tail = _guids.FindById(TAIL_KEY).Value;

        // Seed if empty
        if (_profiles.Count() == 0)
        {
            var rawPresets = presetProfileProvider.Get();
            var presets = rawPresets.DistinctBy(x => x.Id).ToList(); // Remove duplicates

            if (presets.Count() == 0)
            {
                presets.Add(Profile.Default);
            }

            // Set head id
            var headId = presets[0].Id.Value;
            _guids.Insert(new GuidDto(HEAD_KEY, headId));

            var tailIndex = presets.Count() - 1;
            for (var i = 0; i <= tailIndex; i++)
            {
                Guid next = i == tailIndex
                    ? Tail
                    : presets[i + 1].Id.Value;
                _profiles.Insert(presets[i].ToDto(next));
            }
        }
    }

    public void AddOrUpdate(Profile profile)
    {
        var id = profile.Id.Value;
        var savedItem = _profiles.FindById(id);

        if (savedItem != null)
        {
            Save(profile.ToDto(savedItem.NextId));
        }
        else
        {
            var tailItem = _profiles.FindOne(x => x.NextId == Tail)
                ?? throw new CollapsedDatabaseException(INVALID_PROFILE_LINKING);

            tailItem.NextId = id;
            Save(tailItem);

            _profiles.Insert(profile.ToDto(Tail));
        }
    }

    public bool Delete(ProfileId id)
    {
        if (_profiles.Count() == 1) return false;

        var target = _profiles.FindOne(x => x.Id == id.Value);
        if (target == null) return false;

        _profiles.Delete(id.Value);

        // head   : Head に target.next 
        // tail   : 一つ前の next に tail
        // それ以外: 一つ前の next に target.next

        if (target.Id == Head)
        {
            UpdateHead(target.NextId);
            return true;
        }

        var prevTarget = _profiles.FindOne(x => x.NextId == target.Id)
            ?? throw new CollapsedDatabaseException(INVALID_PROFILE_LINKING);

        if (target.NextId == Tail)
        {
            prevTarget.NextId = Tail;
        }
        else
        {
            prevTarget.NextId = target.NextId;
        }
        Save(prevTarget);
        return true;
    }

    public Profile? Get(ProfileId id)
    {
        return _profiles.FindById(id.Value)?.ToModel();
    }

    public List<Profile> GetAll()
    {
        var list = new List<Profile>();
        Guid cursor = Head;

        while (cursor != Tail)
        {
            var profileDto = _profiles.FindById(cursor);
            if (profileDto == null) throw new CollapsedDatabaseException(INVALID_PROFILE_LINKING);
            list.Add(profileDto.ToModel());
            cursor = profileDto.NextId;
        }

        return list;
    }

    public void MoveIndex(ProfileId id, int count)
    {
        if (count == 0) return;
        if (count < 0)
        {
            for (int i = 0; i < -count; i++)
            {
                if (!MoveBackward(id.Value)) break;
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                if (!MoveForward(id.Value)) break;
            }
        }
    }

    private void UpdateHead(Guid guid) => _guids.Update(new GuidDto(HEAD_KEY, guid));

    private bool MoveBackward(Guid id)
    {
        // prevPrevTarget -> prevTarget -> target
        //                       |
        //                       v
        // prevPrevTarget --> target --> prevTarget

        if (id == Head) return false;

        var target = _profiles.FindById(id);
        if (target == null) return false;

        var prevTarget = _profiles.FindOne(x => x.NextId == id)
            ?? throw new CollapsedDatabaseException(INVALID_PROFILE_LINKING);

        if (prevTarget.Id == Head)
        {
            UpdateHead(id);
            prevTarget.NextId = target.NextId;
            target.NextId = prevTarget.Id;
            Save(prevTarget, target);
            return true;
        }

        var prevPrevTarget = _profiles.FindOne(x => x.NextId == prevTarget.Id)
            ?? throw new CollapsedDatabaseException(INVALID_PROFILE_LINKING);

        prevPrevTarget.NextId = target.Id;
        prevTarget.NextId = target.NextId;
        target.NextId = prevTarget.Id;
        Save(prevPrevTarget, prevTarget, target);
        return true;
    }

    private bool MoveForward(Guid id)
    {
        // prevTarget -> target -> nextTarget
        //                  |
        //                  v
        // prevTarget --> nextTarget --> target

        var target = _profiles.FindById(id);
        if (target == null) return false;

        if (target.NextId == Tail) return false;
        var nextTarget = _profiles.FindById(target.NextId)
            ?? throw new CollapsedDatabaseException(INVALID_PROFILE_LINKING);

        if (target.Id == Head)
        {
            UpdateHead(nextTarget.Id);
            target.NextId = nextTarget.NextId;
            nextTarget.NextId = target.Id;
            Save(target, nextTarget);
            return true;
        }

        var prevTarget = _profiles.FindOne(x => x.NextId == target.Id)
            ?? throw new CollapsedDatabaseException(INVALID_PROFILE_LINKING);

        prevTarget.NextId = nextTarget.Id;
        target.NextId = nextTarget.NextId;
        nextTarget.NextId = target.Id;
        Save(target, prevTarget, nextTarget);
        return true;
    }

    private void Save(params ProfileDto[] profiles)
    {
        foreach (var profileDto in profiles)
        {
            _profiles.Update(profileDto);
        }
    }

    public ProfileId GetActiveProfileId()
    {
        var activeId = _guids.FindById(ACTIVE_KEY);

        // seed
        if (activeId == null)
        {
            activeId = _guids.FindById(HEAD_KEY);
            _guids.Insert(new GuidDto(ACTIVE_KEY, activeId.Value));
        }

        // profile deleted
        if (_profiles.FindById(activeId.Value) == null)
        {
            activeId = _guids.FindById(HEAD_KEY);
            _guids.Update(new GuidDto(ACTIVE_KEY, activeId.Value));

            if (_profiles.FindById(activeId.Value) == null) throw new CollapsedDatabaseException(INVALID_PROFILE_LINKING);
        }

        return new(activeId.Value);
    }

    public bool SetActiveProfile(ProfileId id)
    {
        if (_profiles.FindById(id.Value) == null) return false;

        if (_guids.FindById(ACTIVE_KEY) == null)
        {
            _guids.Insert(new GuidDto(ACTIVE_KEY, id.Value));
        }
        else
        {
            _guids.Update(new GuidDto(ACTIVE_KEY, id.Value));
        }

        return true;
    }
}