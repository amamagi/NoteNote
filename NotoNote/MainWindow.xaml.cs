using NotoNote.Services;
using NotoNote.ViewModels;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using Application = System.Windows.Application;

namespace NotoNote;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel ViewModel;
    private HotKeyService? _hotKeyService;

    private NotifyIcon notifyIcon = new();
    private readonly Dictionary<string, ToolStripMenuItem> _toolStripMenuItems = [];

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        _hotKeyService = new HotKeyService(
            this,
            HotKeyService.Modifiers.Ctrl | HotKeyService.Modifiers.Shift,
            (uint)KeyInterop.VirtualKeyFromKey(Key.Space));
        _hotKeyService.HotKeyPressed += (_, _) => ViewModel.ToggleRecordingCommand.Execute(null);
    }

    private void SetNotifyIcon()
    {
        var items = new List<string> { "終了" };
        var toolStripMenu = new ContextMenuStrip();
        foreach ( var item in items)
        {
            if (item != "-")
            {
                _toolStripMenuItems[item] = new ToolStripMenuItem()
                {
                    Text = item,
                    Image = null
                };
                toolStripMenu.Items.Add(_toolStripMenuItems[item]);
                _toolStripMenuItems[item].Click += (s, e) =>
                {
                    var method = GetType().GetMethod(item.Replace(" ", "") + "ToolStripMenuItem_Click", BindingFlags.Instance | BindingFlags.NonPublic);
                    method?.Invoke(this, [s, e]);
                };
            }
            else
            {
                toolStripMenu.Items.Add(new ToolStripSeparator());
            }
        }
        var assembly = Assembly.GetExecutingAssembly(); 
        var icon = Application.GetResourceStream(new Uri("icon.ico", UriKind.Relative)).Stream;
        notifyIcon = new NotifyIcon()
        {
            Text = "Test",
            Visible = true,
            Icon = new Icon(icon),
            ContextMenuStrip = toolStripMenu,
        };
    }

    private void ExitToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        notifyIcon?.Dispose();
        Application.Current.Shutdown();
    }

    public MainWindow(MainViewModel vm)
    {
        InitializeComponent();
        SetNotifyIcon();
        DataContext = vm;
        ViewModel = vm;
    }
}