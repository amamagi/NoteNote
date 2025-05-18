using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using System.Collections.Specialized;

namespace NotoNote.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IProfileRepository _profilesRepository;
    private readonly IApiKeyRepository _apiKeyRepository;

    public class Hotkey
    {
        public Keys Key { get; set; }
        public bool Shift { get; set; }
        public bool Ctrl { get; set; }
        public bool Alt { get; set; }
    }

    [ObservableProperty] private string _openAiApiKey = string.Empty; // Initialize to avoid nullability issue
    [ObservableProperty] private Hotkey _hotkeyActivation;
    [ObservableProperty] private Hotkey _hotkeyToggleProfile;

    public Keys[] AvailableKeys => Constants.AvailableKeys;

    [ObservableProperty] List<Profile> _profiles = [];
    [ObservableProperty] Profile _selectedProfile;

    public SettingsViewModel(IProfileRepository profiles, IApiKeyRepository apiKey)
    {
        _profilesRepository = profiles;
        _apiKeyRepository = apiKey;

        _hotkeyActivation = new Hotkey()
        {
            Key = Keys.S,
            Shift = true
        };
        _hotkeyToggleProfile = new Hotkey()
        {
            Key = Keys.Tab,
            Shift = false
        };

        var savedOpenAiApiKey = _apiKeyRepository.Get(ApiProvider.OpenAI);
        if (savedOpenAiApiKey != null) OpenAiApiKey = savedOpenAiApiKey.Value;

        _profiles = _profilesRepository.GetAll();
        _selectedProfile = _profiles[0];
    }

    partial void OnOpenAiApiKeyChanged(string value)
    {
        if (string.IsNullOrEmpty(value)) _apiKeyRepository.Delete(ApiProvider.OpenAI);
        else _apiKeyRepository.AddOrUpdate(new ApiKey(ApiProvider.OpenAI, value));
    }

    [RelayCommand]
    private void AddProfile()
    {
        var newProfile = new Profile(
            new(Guid.NewGuid()),
            new("NewProfile"),
            new(""),
            Constants.AvailableTranscriptionAiModels[0].Id,
            Constants.AvailableChatAiModels[0].Id);
        _profilesRepository.AddOrUpdate(newProfile);
        Profiles = _profilesRepository.GetAll();
        SelectedProfile = Profiles[^1];
    }


    [RelayCommand]
    private void DeleteProfile()
    {
        if (SelectedProfile == null) return;

        var allProfiles = _profilesRepository.GetAll();
        if (allProfiles.Count == 1) return;

        var index = allProfiles.IndexOf(SelectedProfile);

        var isLast = index == allProfiles.Count - 1;
        _profilesRepository.Delete(SelectedProfile.Id);

        Profiles = _profilesRepository.GetAll();
        SelectedProfile = Profiles[isLast ? index - 1 : index];
    }


    [RelayCommand]
    private void MoveProfileToUp()
    {
        if (SelectedProfile == null) return;
        var allProfiles = _profilesRepository.GetAll();
        if (allProfiles.Count == 1) return;
        var index = allProfiles.IndexOf(SelectedProfile);
        if (index == 0) return;
        _profilesRepository.MoveIndex(SelectedProfile.Id, -1);

        Profiles = _profilesRepository.GetAll();
        SelectedProfile = Profiles[index - 1];
    }


    [RelayCommand]
    private void MoveProfileToDown()
    {
        if (SelectedProfile == null) return;
        var allProfiles = _profilesRepository.GetAll();
        if (allProfiles.Count == 1) return;
        var index = allProfiles.IndexOf(SelectedProfile);
        if (index == allProfiles.Count - 1) return;
        _profilesRepository.MoveIndex(SelectedProfile.Id, 1);

        Profiles = _profilesRepository.GetAll();
        SelectedProfile = Profiles[index + 1];
    }
}
