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
    //private void ProfileName_LostFocus(object sender, RoutedEventArgs e)
    //{
    //    if (DataContext is SettingsViewModel viewModel && sender is System.Windows.Controls.TextBox textBox)
    //    {
    //        if (viewModel.SelectedProfile == null) return;

    //        if (viewModel.SelectedProfile.Name.Value != textBox.Text)
    //        {
    //            var newProfile = viewModel.SelectedProfile with { Name = new ProfileName(textBox.Text) };
    //            viewModel.UpdateSelectedProfile(newProfile);
    //        }
    //    }

    //}

}
