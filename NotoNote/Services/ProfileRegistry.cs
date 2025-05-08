using Microsoft.Extensions.Options;
using NotoNote.Models;
using System.Diagnostics;

namespace NotoNote.Services;

public sealed class ProfileRegistry : IProfileRegistry
{
    public ProfileRegistry(IOptions<List<ProfileOptions>> options)
    {
        var profiles = options.Value.ToArray();


        if (profiles == null || profiles.Length == 0)
        {
            throw new ArgumentException("No profiles found in configuration.");
        }

        foreach (var profile in profiles)
        {
            var profileName = profile.Name;
            var systemPrompt = string.Join("\n", profile.SystemPrompts);
            Profiles.Add(new Profile(
                new ProfileName(profileName),
                new SystemPrompt(systemPrompt),
                Constants.AvailableTranscriptionAiModels.First().Id,
                Constants.AvailableChatAiModels.First().Id));
        }

    }

    public List<Profile> Profiles { get; } = new();

}
