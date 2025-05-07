using System.Windows;

namespace NotoNote.Models;

public interface IHotKeyService
{
    // FIXME: anti pattern
    public void LazyInit(Window window, uint modifiers, uint key);
    public void SetCallback(Action action);
    public void Clean();
}
