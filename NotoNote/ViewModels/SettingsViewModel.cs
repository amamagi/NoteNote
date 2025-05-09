using CommunityToolkit.Mvvm.ComponentModel;
using NotoNote.Models;

namespace NotoNote.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private IEnumerable<Profile> _profiles;

    public SettingsViewModel(IProfileRepository profiles)
    {
        Profiles = profiles.GetAll();
    }
}
