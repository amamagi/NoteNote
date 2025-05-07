using NotoNote.Models;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace NotoNote.Services;
public sealed class HotKeyService : IHotKeyService, IDisposable
{
    private Window _window;
    private int _id;
    private HwndSource _source;

    public event EventHandler? HotKeyPressed;

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        const int WH_HOTKEY = 0x0312;
        if (msg == WH_HOTKEY && wParam.ToInt32() == _id)
        {
            HotKeyPressed?.Invoke(this, EventArgs.Empty);
            handled = true;
        }
        return IntPtr.Zero;
    }

    public void Dispose()
    {
        _source.RemoveHook(HwndHook);
        NativeMethods.UnregisterHotKey(_source.Handle, _id);

    }

    public void LazyInit(Window window, uint modifiers, uint key)
    {
        _window = window;
        _id = GetHashCode();
        _source = (HwndSource)PresentationSource.FromVisual(window);
        _source.AddHook(HwndHook);

        bool ok = NativeMethods.RegisterHotKey(_source.Handle, _id, modifiers, key);
        if (!ok) throw new Win32Exception();
    }

    public void SetCallback(Action action)
    {
        HotKeyPressed += (_, _) => action();
    }

    public void Clean()
    {
        HotKeyPressed = null;
    }

    public static class Modifiers
    {
        public const uint Ctrl = 0x0002;
        public const uint Shift = 0x0004;
    }
}

