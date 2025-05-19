using NotoNote.DataStore;

namespace NotoNote.Models;
public interface IHotkeyRepository
{
    public void Delete(HotkeyPurpose purpose);
    public Hotkey Get(HotkeyPurpose purpose);
    public void Update(HotkeyPurpose purpose, Hotkey hotkey);
    public void Set(HotkeyPurpose purpose, Hotkey hotkey);
    public Dictionary<HotkeyPurpose, Hotkey> GetAll();
}
