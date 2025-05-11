using CommunityToolkit.Mvvm.ComponentModel;
using NotoNote.Models;
using System.Diagnostics;

namespace NotoNote.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IProfileRepository _profileRepository;
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

    public Keys[] AvailableKeys => Constants.AvailableKeys;

    public SettingsViewModel(IProfileRepository profiles, IApiKeyRepository apiKey)
    {
        _profileRepository = profiles;
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
