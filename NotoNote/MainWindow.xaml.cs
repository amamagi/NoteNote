using NotoNote.Service;
using NotoNote.ViewModels;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace NotoNote;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel ViewModel;
    private HotKeyService? _hotKeyService;

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        _hotKeyService = new HotKeyService(
            this,
            HotKeyService.Modifiers.Ctrl | HotKeyService.Modifiers.Shift,
            (uint)KeyInterop.VirtualKeyFromKey(Key.Space));
        _hotKeyService.HotKeyPressed += (_, _) => ViewModel.ToggleRecordingCommand.Execute(null);
    }

    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
        ViewModel = vm;
    }
}