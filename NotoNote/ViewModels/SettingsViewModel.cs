using CommunityToolkit.Mvvm.ComponentModel;
using NotoNote.Models;

namespace NotoNote.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IProfileRepository _profileRepository;

    public class Hotkey
    {
        public char Key { get; set; }
        public bool Shift { get; set; }
        public bool Ctrl { get; set; }
        public bool Alt { get; set; }
    }

    [ObservableProperty] private string _openAiApiKey;
    [ObservableProperty] private Hotkey _hotkeyActivation;
    [ObservableProperty] private Hotkey _hotkeyToggleProfile;

    public SettingsViewModel(IProfileRepository profiles)
    {
        _profileRepository = profiles;

        _hotkeyActivation = new Hotkey()
        {
            Key = "a"[0],
            Shift = true
        };
    }
}
