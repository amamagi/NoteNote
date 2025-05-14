using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using System.Diagnostics;

namespace NotoNote.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IProfileRepository _profilesRepository;
    private readonly IApiKeyRepository _apiKeyRepository;

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
    private List<Profile> _profiles;

    public Keys[] AvailableKeys => Constants.AvailableKeys;

    public List<Profile> Profiles => _profiles;

    [ObservableProperty] Profile _selectedProfile;

    public SettingsViewModel(IProfileRepository profiles, IApiKeyRepository apiKey)
    {
        _profilesRepository = profiles;
        _apiKeyRepository = apiKey;

        _hotkeyActivation = new Hotkey()
        {
            Key = "a"[0],
            Shift = true
        };
        _hotkeyToggleProfile = new Hotkey()
        {
            Key = "b"[0],
            Shift = false
        };

        var savedOpenAiApiKey = _apiKeyRepository.Get(ApiProvider.OpenAI);
        if (savedOpenAiApiKey != null) OpenAiApiKey = savedOpenAiApiKey.Value;

        _profiles = _profilesRepository.GetAll().ToList();
        _selectedProfile = _profiles[0];
    }

    partial void OnOpenAiApiKeyChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            _apiKeyRepository.Delete(ApiProvider.OpenAI);
        }
        else
        {
            var apiKey = new ApiKey(ApiProvider.OpenAI, value);
            if (_apiKeyRepository.Get(ApiProvider.OpenAI) != null)
            {
                _apiKeyRepository.Update(apiKey);
            }
            else
            {
                _apiKeyRepository.Set(apiKey);
            }
        }
    }

}
