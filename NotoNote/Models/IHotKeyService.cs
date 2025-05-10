namespace NotoNote.Models;

public interface IHotkeyService
{
    public void RegisterHotkey(Hotkey hotkey, Action callback);
    public void UnregisterHotkey(Hotkey hotkey);
}
