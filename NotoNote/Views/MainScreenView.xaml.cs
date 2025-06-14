﻿using NotoNote.ViewModels;
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
/// Interaction logic for MainScreenView.xaml
/// </summary>
public partial class MainScreenView : System.Windows.Controls.UserControl
{
    public MainScreenView(MainScreenViewModel vm, SettingsView settingView)
    {
        InitializeComponent();
        DataContext = vm;
        SettingsHost.Content = settingView;
    }
}
