using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace NotoNote.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IProfileRepository _profilesRepository;
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IHotkeyRepository _hotkeyRepository;
    private readonly Dictionary<HotkeyPurpose, string> _hotkeyText = new()
    {
        { HotkeyPurpose.Activation, ""},
        { HotkeyPurpose.ToggleProfile, ""}
    };

    private readonly Dictionary<HotkeyPurpose, string> _hotkeyTextProperty = new()
    {
        { HotkeyPurpose.Activation, nameof(HotkeyActivationText) },
        { HotkeyPurpose.ToggleProfile, nameof(HotkeyToggleProfileText) }
    };

    [ObservableProperty]
    private string _openAiApiKey = string.Empty;

    public static Keys[] AvailableKeys => Constants.AvailableKeys;
    public TranscriptionAiModelId[] AvailableTranscriptionAiModels { get; } = Constants.AvailableTranscriptionAiModels.Select(m => m.Id).ToArray();
    public ChatAiModelId[] AvailableChatAiModels { get; } = Constants.AvailableChatAiModels.Select(m => m.Id).ToArray();

    [ObservableProperty] List<Profile> _profiles = [];
    [ObservableProperty] Profile _selectedProfile;

    public string HotkeyActivationText => _hotkeyText[HotkeyPurpose.Activation];
    public string HotkeyToggleProfileText => _hotkeyText[HotkeyPurpose.ToggleProfile];
    public string SelectedProfileName
    {
        get => SelectedProfile?.Name.Value ?? string.Empty;
        set
        {
            if (SelectedProfile == null) return;
            var newProfile = SelectedProfile with { Name = new ProfileName(value) };
            UpdateSelectedProfile(newProfile);
        }
    }

    public string SelectedProfileSystemPrompt
    {
        get => SelectedProfile?.SystemPrompt.Value ?? string.Empty;
        set
        {
            if (SelectedProfile == null) return;
            var newProfile = SelectedProfile with { SystemPrompt = new SystemPrompt(value) };
            UpdateSelectedProfile(newProfile);
        }
    }

    public TranscriptionAiModelId SelectedTranscriptionAiModelId
    {
        get => SelectedProfile?.TranscriptionModelId ?? Constants.AvailableTranscriptionAiModels[0].Id;
        set
        {
            if (SelectedProfile == null) return;
            var newProfile = SelectedProfile with { TranscriptionModelId = value };
            UpdateSelectedProfile(newProfile);
        }
    }

    public ChatAiModelId SelectedChatAiModelId
    {
        get => SelectedProfile?.ChatModelId ?? Constants.AvailableChatAiModels[0].Id;
        set
        {
            if (SelectedProfile == null) return;
            var newProfile = SelectedProfile with { ChatModelId = value };
            UpdateSelectedProfile(newProfile);
        }
    }

    public Dictionary<HotkeyPurpose, string> HotkeyTextProperty => _hotkeyTextProperty;

    public SettingsViewModel(IProfileRepository profiles, IApiKeyRepository apiKey, IHotkeyRepository hotkeyRepository)
    {
        _profilesRepository = profiles;
        _apiKeyRepository = apiKey;
        _hotkeyRepository = hotkeyRepository;

        // API Key
        var savedOpenAiApiKey = _apiKeyRepository.Get(ApiProvider.OpenAI);
        if (savedOpenAiApiKey != null) OpenAiApiKey = savedOpenAiApiKey.Value;

        // Profile
        _profiles = _profilesRepository.GetAll();
        var activeId = _profilesRepository.GetActiveProfileId();
        _selectedProfile = _profiles.Find(x => x.Id == activeId) ?? _profiles[0];

        // Hotkey
        if (_hotkeyRepository.Get(HotkeyPurpose.Activation) is { } activationHotkey)
        {
            UpdateHotkeyText(HotkeyPurpose.Activation, activationHotkey);
        }
        if (_hotkeyRepository.Get(HotkeyPurpose.ToggleProfile) is { } toggleHotkey)
        {
            UpdateHotkeyText(HotkeyPurpose.ToggleProfile, toggleHotkey);
        }
    }
    public void UpdateSelectedProfile(Profile updatedProfile)
    {
        _profilesRepository.AddOrUpdate(updatedProfile);

        // リストの該当項目のみ更新
        var index = Profiles.FindIndex(p => p.Id.Value == updatedProfile.Id.Value);
        if (index >= 0)
        {
            Profiles[index] = updatedProfile;
            OnPropertyChanged(nameof(Profiles));
        }

        SelectedProfile = updatedProfile;
    }

    // XAMLのバインディングはプロパティのプロパティ変更イベントを拾えないので、
    // ViewModelに直接値を持ってSelectedProfileの更新にフックさせて変更イベントを拾う
    partial void OnSelectedProfileChanged(Profile value)
    {
        OnPropertyChanged(nameof(SelectedProfileName));
        OnPropertyChanged(nameof(SelectedProfileSystemPrompt));
        OnPropertyChanged(nameof(SelectedTranscriptionAiModelId));
        OnPropertyChanged(nameof(SelectedChatAiModelId));
    }

    partial void OnOpenAiApiKeyChanged(string value)
    {
        if (string.IsNullOrEmpty(value)) _apiKeyRepository.Delete(ApiProvider.OpenAI);
        else _apiKeyRepository.AddOrUpdate(new ApiKey(ApiProvider.OpenAI, value));
    }

    [RelayCommand]
    private void AddProfile()
    {
        _profilesRepository.AddOrUpdate(Profile.Default);
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

    public void UpdateHotkey(HotkeyPurpose purpose, Key key, bool isShiftPressed, bool isCtrlPressed, bool isAltPressed)
    {
        var modifiers = Keys.None;
        if (isShiftPressed) modifiers |= Keys.Shift;
        if (isCtrlPressed) modifiers |= Keys.Control;
        if (isAltPressed) modifiers |= Keys.Alt;
        var virtualKey = (Keys)KeyInterop.VirtualKeyFromKey(key);
        if (virtualKey == Keys.None) return;
        if (modifiers == Keys.None) return; // No modifiers, no hotkey
        var hotkey = new Hotkey(virtualKey, modifiers);

        // Validate the hotkey
        var allHotkeys = _hotkeyRepository.GetAll();
        foreach (var existingHotkey in allHotkeys)
        {
            if (existingHotkey.Key != purpose && existingHotkey.Value == hotkey)
            {
                // Show a message to the user about the conflict
                MessageBox.Show("This hotkey is already in use. Please choose a different one.", "Hotkey Conflict", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        _hotkeyRepository.Update(purpose, hotkey);
        UpdateHotkeyText(purpose, hotkey);
    }

    public void UpdateHotkeyText(HotkeyPurpose purpose, Hotkey hotkey)
    {
        _hotkeyText[purpose] = hotkey.Modifiers.ToString().Replace(", ", "+") + "+" + hotkey.Key.ToString();
        OnPropertyChanged(_hotkeyTextProperty[purpose]);
    }
}
