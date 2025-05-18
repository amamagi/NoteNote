using NotoNote.Models;
using Open.WinKeyboardHook;
using System.Windows.Threading;

namespace NotoNote.Services;
public sealed class HotkeyService : IHotkeyService, IDisposable
{
    private readonly Dictionary<Hotkey, Action> _hotkeyCallbacks = [];
    private readonly IKeyboardInterceptor _interceptor;
    private readonly Dispatcher _uiDispatcher;

    public HotkeyService(Dispatcher uiDispatcher)
    {
        _uiDispatcher = uiDispatcher;
        _interceptor = new KeyboardInterceptor();
        _interceptor.KeyDown += InterceptorOnKeyDown;
        _interceptor.StartCapturing();
    }

    public void Dispose()
    {
        _interceptor?.StopCapturing();
    }

    public void RegisterHotkey(Hotkey hotkey, Action callback)
    {
        _hotkeyCallbacks[hotkey] = callback;
    }

    public void UnregisterHotkey(Hotkey hotkey)
    {
        _hotkeyCallbacks.Remove(hotkey);
    }

    public void UnregisterAllHotkeys()
    {
        _hotkeyCallbacks.Clear();
    }

    private void InterceptorOnKeyDown(object? sender, KeyEventArgs args)
    {
        var keys = args.KeyCode & ~Keys.Modifiers;
        var modifiers = args.Modifiers;
        var hotkey = new Hotkey(keys, modifiers);
        var handled = InvokeCallbacks(hotkey, _hotkeyCallbacks);
        if (handled) args.SuppressKeyPress = true;
    }

    private bool InvokeCallbacks(Hotkey hotkey, Dictionary<Hotkey, Action> callbacks)
    {
        bool handled = false;
        foreach (var callback in callbacks)
        {
            if (callback.Key == hotkey)
            {
                handled = true;
                _uiDispatcher.BeginInvoke(callback.Value);
            }
        }
        return handled;
    }

}

