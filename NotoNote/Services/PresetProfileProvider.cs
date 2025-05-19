using Microsoft.Extensions.Options;
using NotoNote.Models;

namespace NotoNote.Services;
public sealed class PresetProfileProvider : IPresetProfileProvider
{
    private readonly List<Profile> _profiles = [];

    public PresetProfileProvider(IOptions<List<PresetProfileOptions>> options)
    {
        var optionProfiles = options.Value;
        if (optionProfiles == null || optionProfiles.Count == 0)
        {
            throw new ArgumentException("No profiles found in configuration.");
        }

        foreach (var optionProfile in optionProfiles)
        {
            var profileName = optionProfile.Name;
            var systemPrompt = string.Join("\n", optionProfile.SystemPrompts);
            var profile = new Profile(
                new ProfileId(Guid.Parse(optionProfile.Id)), // TODO: Use option value
                new ProfileName(profileName),
                new SystemPrompt(systemPrompt),
                Constants.AvailableTranscriptionModels[0].Id, // TODO: Use option value
                Constants.AvailableChatModels[0].Id); // TODO: Use option value
            _profiles.Add(profile);
        }
    }

    public List<Profile> Get()
    {
        return _profiles;
    }
}