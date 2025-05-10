using NotoNote.Models;
using Open.WinKeyboardHook;
using System.Windows.Threading;

namespace NotoNote.Services;
public sealed class HotkeyService : IHotkeyService, IDisposable
{

    private readonly Dictionary<Hotkey, Action> _keyUpCallbacks = [];
    private IKeyboardInterceptor? _interceptor;
    private Dispatcher _uiDispatcher;

    public HotkeyService(Dispatcher uiDispatcher)
    {
        _uiDispatcher = uiDispatcher;
        _interceptor = new KeyboardInterceptor();
        _interceptor.KeyUp += InterceptorOnKeyUp;
        _interceptor.StartCapturing();
    }

    public void Dispose()
    {
        _interceptor?.StopCapturing();
    }

    public void RegisterHotkey(Hotkey hotkey, Action callback)
    {
        _keyUpCallbacks[hotkey] = callback;
    }

    public void UnregisterHotkey(Hotkey hotkey)
    {
        _keyUpCallbacks.Remove(hotkey);
    }

    private void InterceptorOnKeyUp(object? sender, KeyEventArgs args)
    {
        var keys = args.KeyCode & ~Keys.Modifiers;
        var modifiers = args.Modifiers;
        var hotkey = new Hotkey(keys, modifiers);
        var handled = InvokeCallbacks(hotkey, _keyUpCallbacks);
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

