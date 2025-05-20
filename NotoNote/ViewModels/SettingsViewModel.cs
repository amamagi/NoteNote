using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;

namespace NotoNote.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IProfileRepository _profilesRepository;
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly IHotkeyRepository _hotkeyRepository;
    private readonly ITranscriptionModelProvider _transcriptionModelProvider;
    private readonly IChatModelProvider _chatModelProvider;

    // 入力したHotkeyの表示用テキスト
    public string HotkeyActivationText => _hotkeyText[HotkeyPurpose.Activation];
    public string HotkeyToggleProfileText => _hotkeyText[HotkeyPurpose.ToggleProfile];

    private readonly Dictionary<HotkeyPurpose, string> _hotkeyText = new()
    {
        { HotkeyPurpose.Activation, ""},
        { HotkeyPurpose.ToggleProfile, ""}
    };
    private readonly IReadOnlyDictionary<HotkeyPurpose, string> _hotkeyTextProperty = new Dictionary<HotkeyPurpose, string>()
    {
        { HotkeyPurpose.Activation, nameof(HotkeyActivationText) },
        { HotkeyPurpose.ToggleProfile, nameof(HotkeyToggleProfileText) }
    };

    [ObservableProperty] private Profile _selectedProfile;
    [ObservableProperty] private ObservableCollection<Profile> _profiles;

    // TODO: ModelId -> ModelName  
    public ITranscriptionModel[] AvailableTranscriptionAiModels { get; }
    public IChatModel[] AvailableChatAiModels { get; }

    public string OpenAiApiKey
    {
        get => _apiKeyRepository.Get(ApiSource.OpenAI)?.Value ?? string.Empty;
        set
        {
            if (string.IsNullOrEmpty(value)) _apiKeyRepository.Delete(ApiSource.OpenAI);
            else _apiKeyRepository.AddOrUpdate(new ApiKey(ApiSource.OpenAI, value));
        }
    }
    public string GeminiApiKey
    {
        get => _apiKeyRepository.Get(ApiSource.Gemini)?.Value ?? string.Empty;
        set
        {
            if (string.IsNullOrEmpty(value)) _apiKeyRepository.Delete(ApiSource.Gemini);
            else _apiKeyRepository.AddOrUpdate(new ApiKey(ApiSource.Gemini, value));
        }
    }

    public string SelectedProfileName
    {
        get => SelectedProfile?.Name.Value ?? string.Empty;
        set
        {
            if (SelectedProfile == null) return;
            var newProfile = SelectedProfile with { Name = new ProfileName(value) };
            UpdateProfiles(newProfile);
        }
    }

    public string SelectedProfileSystemPrompt
    {
        get => SelectedProfile?.SystemPrompt.Value ?? string.Empty;
        set
        {
            if (SelectedProfile == null) return;
            var newProfile = SelectedProfile with { SystemPrompt = new SystemPrompt(value) };
            UpdateProfiles(newProfile);
        }
    }

    public ITranscriptionModel SelectedTranscriptionAiModel
    {
        get
        {
            if (SelectedProfile != null && SelectedProfile.TranscriptionModelId != null)
            {
                var selected = _transcriptionModelProvider.Get(SelectedProfile.TranscriptionModelId);
                if (selected != null) return selected;
            }
            return _transcriptionModelProvider.GetAll().First();
        }
        set
        {
            if (SelectedProfile == null) return;
            var newProfile = SelectedProfile with { TranscriptionModelId = value.Id };
            UpdateProfiles(newProfile);
        }
    }

    public IChatModel SelectedChatAiModel
    {
        get
        {
            if (SelectedProfile != null && SelectedProfile.ChatModelId != null)
            {
                var selected = _chatModelProvider.Get(SelectedProfile.ChatModelId);
                if (selected != null) return selected;
            }
            return _chatModelProvider.GetAll().First();
        }
        set
        {
            if (SelectedProfile == null) return;
            var newProfile = SelectedProfile with { ChatModelId = value.Id };
            UpdateProfiles(newProfile);
        }
    }

    public SettingsViewModel(
        IProfileRepository profiles,
        IApiKeyRepository apiKey,
        IHotkeyRepository hotkeyRepository,
        ITranscriptionModelProvider transcriptionModelProvider,
        IChatModelProvider chatModelProvider)
    {
        _profilesRepository = profiles;
        _apiKeyRepository = apiKey;
        _hotkeyRepository = hotkeyRepository;
        _transcriptionModelProvider = transcriptionModelProvider;
        _chatModelProvider = chatModelProvider;

        AvailableTranscriptionAiModels = transcriptionModelProvider.GetAll().ToArray();
        AvailableChatAiModels = chatModelProvider.GetAll().ToArray();

        // Profile
        var activeId = _profilesRepository.GetActiveProfileId();
        _selectedProfile = _profilesRepository.Get(activeId) ?? _profilesRepository.GetAll().First();
        Profiles = new ObservableCollection<Profile>(_profilesRepository.GetAll());

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

    public void UpdateProfiles(Profile updatedProfile)
    {
        Debug.WriteLine($"UpdateProfiles: {updatedProfile.Id} {updatedProfile.Name} {updatedProfile.SystemPrompt} {updatedProfile.TranscriptionModelId} {updatedProfile.ChatModelId}");

        _profilesRepository.AddOrUpdate(updatedProfile);

        // update list item source
        var existProfile = Profiles.FirstOrDefault(p => p.Id == updatedProfile.Id);
        if (existProfile != null)
        {
            var index = Profiles.IndexOf(existProfile);
            Profiles[index] = updatedProfile;
        }
        else
        {
            Profiles.Add(updatedProfile);
        }

        SelectedProfile = updatedProfile;
    }

    // XAMLのバインディングはプロパティのプロパティ変更イベントを拾えないので、
    // ViewModelに直接値を持ってSelectedProfileの更新にフックさせて変更イベントを拾わせる
    partial void OnSelectedProfileChanged(Profile value)
    {
        OnPropertyChanged(nameof(SelectedProfileName));
        OnPropertyChanged(nameof(SelectedProfileSystemPrompt));
        OnPropertyChanged(nameof(SelectedTranscriptionAiModel));
        OnPropertyChanged(nameof(SelectedChatAiModel));
    }

    [RelayCommand]
    private void AddProfile()
    {
        UpdateProfiles(Profile.Default);
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
        Profiles.Remove(SelectedProfile);
        SelectedProfile = Profiles[isLast ? index - 1 : index];
    }


    [RelayCommand]
    private void MoveProfileToUp()
    {
        if (SelectedProfile == null) return;

        var allProfiles = _profilesRepository.GetAll();
        if (allProfiles.Count == 1) return;

        // 最上位は移動できない
        var index = allProfiles.IndexOf(SelectedProfile);
        if (index == 0) return;

        _profilesRepository.MoveIndex(SelectedProfile.Id, -1);
        (Profiles[index], Profiles[index - 1]) = (Profiles[index - 1], Profiles[index]);
        SelectedProfile = Profiles[index - 1];
    }


    [RelayCommand]
    private void MoveProfileToDown()
    {
        if (SelectedProfile == null) return;

        var allProfiles = _profilesRepository.GetAll();
        if (allProfiles.Count == 1) return;

        // 最下位は移動できない
        var index = allProfiles.IndexOf(SelectedProfile);
        if (index == allProfiles.Count - 1) return;

        _profilesRepository.MoveIndex(SelectedProfile.Id, 1);
        (Profiles[index], Profiles[index + 1]) = (Profiles[index + 1], Profiles[index]);
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
