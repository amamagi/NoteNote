using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static NotoNote.Services.NativeMethods;
using Clipboard = System.Windows.Clipboard;
using IDataObject = System.Windows.IDataObject;

namespace NotoNote.Services;

public sealed class ClipBoardService
{
    [DllImport("user32")] static extern bool SetForegroundWindow(IntPtr hWnd);
    public static async Task<bool> PastToCurrentCaret(string text)
    {
        if (!IsCaretExist()) return false;

        IDataObject? old = Clipboard.GetDataObject();

        Clipboard.SetText(text);

        SendCtrlV();

        await Task.Delay(200);

        if (old != null) Clipboard.SetDataObject(old);

        return true;

    }

    private static bool IsCaretExist()
    {
        // UI Automation で ValuePattern を持ち ReadOnly==false か
        var ae = AutomationElement.FocusedElement;
        if (ae != null &&
            ae.TryGetCurrentPattern(ValuePattern.Pattern, out object? vpObj) &&
            vpObj is ValuePattern vp &&
            vp.Current.IsReadOnly == false)
            return true;

        return false;
    }

    private static void SendCtrlV()
    {
        var inputs = new[]
        {
            NewKey(Key.LeftCtrl, true),
            NewKey(Key.V, true),
            NewKey(Key.V, false),
            NewKey(Key.LeftCtrl, false)
            //NewKey(Key.LWin, true)
        };
        NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<NativeMethods.INPUT>());
    }

    private static INPUT NewKey(Key key, bool down)
    {
        ushort vk = (ushort)KeyInterop.VirtualKeyFromKey(key);
        return new INPUT
        {
            type = 1, // INPUT_KEYBOARD
            u = new INPUTUNION
            {
                ki = new KEYBOARDINPUT
                {
                    wVK = vk,
                    dwFlags = down ? 0u : 2u // KEYEVENTF_KEYUP
                }
            }
        };
    }
}
