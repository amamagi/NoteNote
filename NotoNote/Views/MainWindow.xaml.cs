using NotoNote.Views;
using System.Windows;

namespace NotoNote;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainScreenView content)
    {
        InitializeComponent();
        MainContent.Content = content;
    }
}