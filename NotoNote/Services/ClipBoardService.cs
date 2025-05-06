using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using static NotoNote.Services.NativeMethods;
using Clipboard = System.Windows.Clipboard;

namespace NotoNote.Services;

public sealed class ClipBoardService
{
    public static void Paste(string text)
    {
        Debug.WriteLine("Paste");
        Clipboard.SetText(text);
        SendCtrlV();
        return;
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
}
