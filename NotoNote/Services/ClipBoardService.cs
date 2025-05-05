using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Automation;
using System.Windows.Input;
using static NotoNote.Services.NativeMethods;
using Clipboard = System.Windows.Clipboard;

namespace NotoNote.Services;

public sealed class ClipBoardService
{
    [DllImport("user32.dll")]
    private static extern int GetDesktopWindow();
    [DllImport("user32")] static extern bool SetForegroundWindow(IntPtr hWnd);
    public static async Task<bool> PastToCurrentCaret(string text)
    {
        Debug.WriteLine("Paste");
        Clipboard.SetText(text);
        SendCtrlV();
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

    private static void SendClick()
    {
        var inputs = new[]
        {
            NewMouse(true),
            NewMouse(false)
        };
        NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<INPUT>());
    }

    private static void Release()
    {

        var inputs = new[]
        {
            NewKey(Key.LeftCtrl, false),
            NewKey(Key.LeftShift, false),
            NewKey(Key.Space, false),
        };
        NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<NativeMethods.INPUT>());
    }

    private static void SendCtrlV()
    {
        var inputs = new[] {
            NewKey(Key.LeftCtrl, true, true),
            NewKey(Key.V, true),
            NewKey(Key.V, false),
            NewKey(Key.LeftCtrl, false, true),
        };
        NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<NativeMethods.INPUT>());
    }

    private static INPUT NewKey(Key key, bool down, bool extended = false)
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
                    dwFlags = down
                    ? (extended ? 1u : 0)
                    : (extended ? 3u : 2u)
                }
            }
        };
    }

    private static INPUT NewMouse(bool down)
    {
        return new INPUT
        {
            type = 0,
            u = new INPUTUNION
            {
                mi = new MOUSEINPUT
                {
                    dwFlags = down ? 0x2 : 0x4
                }
            }
        };
    }
}
