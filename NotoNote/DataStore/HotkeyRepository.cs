using LiteDB;
using NotoNote.Models;

namespace NotoNote.DataStore;
public sealed class HotkeyRepository : IHotkeyRepository
{
    private readonly ILiteCollection<HotkeyDto> _collection;
    public HotkeyRepository(ILiteDbContext context)
    {
        _collection = context.Hotkeys;
        Seed();
    }

    private void Seed()
    {
        var activateId = (int)HotkeyPurpose.Activation;
        if (_collection.FindById(activateId) == null)
        {
            _collection.Insert(new HotkeyDto()
            {
                Id = activateId,
                Key = Keys.Space,
                Modifiers = Keys.Shift | Keys.Control
            });
        }
        var toggleProfileId = (int)HotkeyPurpose.ToggleProfile;
        if (_collection.FindById(toggleProfileId) == null)
        {
            _collection.Insert(new HotkeyDto()
            {
                Id = toggleProfileId,
                Key = Keys.S,
                Modifiers = Keys.Shift | Keys.Control
            });
        }
    }

    public void Delete(HotkeyPurpose purpose)
    {
        _collection.Delete((int)purpose);
    }

    public Hotkey Get(HotkeyPurpose purpose)
    {
        var dto = _collection.FindById((int)purpose);
        if (dto == null)
        {
            // recover data
            Seed();
            dto = _collection.FindById((int)purpose);
        }
        return dto.ToModel();
    }

    public void Update(HotkeyPurpose purpose, Hotkey hotkey)
    {
        _collection.Update(hotkey.ToDto(purpose));
    }

    public void Set(HotkeyPurpose purpose, Hotkey hotkey)
    {
        _collection.Insert(hotkey.ToDto(purpose));
    }

    public Dictionary<HotkeyPurpose, Hotkey> GetAll()
    {
        return _collection.FindAll().ToDictionary(x => (HotkeyPurpose)x.Id, x => x.ToModel());
    }
}
