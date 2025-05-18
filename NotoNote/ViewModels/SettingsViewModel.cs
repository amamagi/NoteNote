using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NotoNote.Models;

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
    public TranscriptionAiModelId[] AvailableTranscriptionAiModels { get; } = Constants.AvailableTranscriptionAiModels.Select(m => m.Id).ToArray();
    public ChatAiModelId[] AvailableChatAiModels { get; } = Constants.AvailableChatAiModels.Select(m => m.Id).ToArray();

    [ObservableProperty] List<Profile> _profiles = [];
    [ObservableProperty] Profile _selectedProfile;

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
    public void UpdateSelectedProfile(Profile updatedProfile)
    {
        _profilesRepository.AddOrUpdate(updatedProfile);

        // リストの該当項目のみ更新
        var index = Profiles.FindIndex(p => p.Id.Value == updatedProfile.Id.Value);
        if (index >= 0)
        {
            var updatedList = new List<Profile>(Profiles);
            updatedList[index] = updatedProfile;
            Profiles = updatedList;
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

}
