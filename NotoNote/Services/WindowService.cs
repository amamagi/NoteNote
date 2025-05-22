using NotoNote.Models;

namespace NotoNote.Services;
public sealed class WindowService : IWindowService
{
    public void Activate()
    {
        var w = System.Windows.Application.Current.MainWindow;
        w.Topmost = true;
        w.Topmost = false;
    }

    public void SetTopmost(bool topmost)
    {
        var w = System.Windows.Application.Current.MainWindow;
        w.Topmost = topmost;
    }
}