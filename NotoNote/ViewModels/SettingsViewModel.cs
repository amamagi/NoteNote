using CommunityToolkit.Mvvm.ComponentModel;
using NotoNote.Models;

namespace NotoNote.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IProfileRepository _profileRepository;

    [ObservableProperty]
    private IEnumerable<Profile> _profiles;

    public SettingsViewModel(IProfileRepository profiles)
    {
        _profileRepository = profiles;
    }
}
