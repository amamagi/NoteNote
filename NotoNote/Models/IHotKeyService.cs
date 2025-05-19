namespace NotoNote.Models;

public interface IHotkeyService
{
    /// <summary>
    /// callback is called from ui thread
    /// </summary>
    public void RegisterHotkey(Hotkey hotkey, Action callback);
    /// <summary>
    /// callback is called from ui thread
    /// </summary>
    public void RegisterHotkey(Hotkey hotkey, Func<Task> callback);
    public void UnregisterHotkey(Hotkey hotkey);
    public void UnregisterAllHotkeys();
}
