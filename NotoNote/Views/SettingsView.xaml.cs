using NotoNote.Models;
using NotoNote.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace NotoNote.Views;
/// <summary>
/// Interaction logic for SettingsView.xaml
/// </summary>
public partial class SettingsView : System.Windows.Controls.UserControl
{
    public SettingsView(SettingsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }

    private void HotkeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        // Prevent regular text input
        e.Handled = true;

        if (sender is not TextBox textBox || DataContext is not SettingsViewModel viewModel)
            return;

        // Determine which hotkey to update based on the Tag
        var hotkeyType = textBox.Tag as string ?? "";

        // Get the pressed key
        var key = e.Key;

        // Skip modifier keys when pressed alone
        if (key == Key.LeftCtrl || key == Key.RightCtrl ||
            key == Key.LeftAlt || key == Key.RightAlt ||
            key == Key.LeftShift || key == Key.RightShift)
            return;

        // Determine which modifier keys are pressed
        bool isShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
        bool isCtrlPressed = Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl);
        bool isAltPressed = Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt);

        var purpose = hotkeyType switch
        {
            "Activation" => HotkeyPurpose.Activation,
            "ToggleProfile" => HotkeyPurpose.ToggleProfile,
            _ => throw new ArgumentException("Invalid hotkey type.")
        };

        viewModel.UpdateHotkey(purpose, key, isShiftPressed, isCtrlPressed, isAltPressed);
    }
}
