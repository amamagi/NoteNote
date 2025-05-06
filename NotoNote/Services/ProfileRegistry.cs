using NotoNote.Models;

namespace NotoNote.Services;

public sealed class ProfileRegistry : IProfileRegistry
{
    public ProfileRegistry()
    {
        Profiles = [.. Constants.SampleProfiles];
    }
    public List<Profile> Profiles { get; }
}
