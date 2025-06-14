﻿using NotoNote.Models;
using Open.WinKeyboardHook;
using System.Diagnostics;
using System.Windows.Threading;

namespace NotoNote.Services;
public sealed class HotkeyService : IHotkeyService, IDisposable
{
    private readonly Dictionary<Hotkey, Action> _hotkeyCallbacks = [];
    private readonly Dictionary<Hotkey, Func<Task>> _hotkeyAsyncCallbacks = [];
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
    public void RegisterHotkey(Hotkey hotkey, Func<Task> callback)
    {
        _hotkeyAsyncCallbacks[hotkey] = callback;
    }

    public void UnregisterHotkey(Hotkey hotkey)
    {
        _hotkeyCallbacks.Remove(hotkey);
        _hotkeyAsyncCallbacks.Remove(hotkey);
    }

    public void UnregisterAllHotkeys()
    {
        _hotkeyCallbacks.Clear();
        _hotkeyAsyncCallbacks.Clear();
    }

    private void InterceptorOnKeyDown(object? sender, KeyEventArgs args)
    {
        var keys = args.KeyCode & ~Keys.Modifiers;
        var modifiers = args.Modifiers;
        var hotkey = new Hotkey(keys, modifiers);
        var handled = InvokeCallbacks(hotkey);
        if (handled) args.SuppressKeyPress = true;
        Debug.WriteLine($"Hotkey {hotkey} pressed. Handled: {handled}");
    }

    private bool InvokeCallbacks(Hotkey hotkey)
    {
        bool handled = false;
        foreach (var callback in _hotkeyCallbacks)
        {
            if (callback.Key == hotkey)
            {
                handled = true;
                _uiDispatcher.BeginInvoke(callback.Value);
            }
        }
        foreach (var callback in _hotkeyAsyncCallbacks)
        {
            if (callback.Key == hotkey)
            {
                handled = true;
                _uiDispatcher.BeginInvoke(async () => await callback.Value());
            }
        }
        return handled;
    }

}

