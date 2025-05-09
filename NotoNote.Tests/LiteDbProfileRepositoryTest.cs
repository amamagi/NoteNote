using Moq;
using NotoNote.DataStore;
using NotoNote.Models;
using NotoNote.Services;

namespace NotoNote.Tests;
public sealed class LiteDbProfileRepositoryTest : IDisposable
{
    private readonly string _tempDbPath;
    private readonly LiteDbProfileRepository _repository;

    public LiteDbProfileRepositoryTest()
    {
        // Create a temporary database path
        _tempDbPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".db");
        var presets = new Mock<IPresetProfileProvider>();
        presets.Setup(x => x.Get()).Returns([]);
        _repository = new LiteDbProfileRepository(presets.Object, _tempDbPath);
    }

    public void Dispose()
    {
        _repository?.Dispose();
        // Clean up the temporary database file
        if (File.Exists(_tempDbPath))
        {
            File.Delete(_tempDbPath);
        }
    }

    [Fact]
    public void Add_And_Get_Works()
    {
        // Arrange
        var profile = new Profile(new("TestProfile"), new("TestSystemPrompt\n\n\na"), new("openai-whisper-1"), new("openai-gpt-4o-mini"));
        // Act
        _repository.Add(profile);
        var retrievedProfile = _repository.Get(profile.Id);
        // Assert
        Assert.NotNull(retrievedProfile);
        Assert.Equal(profile.Id, retrievedProfile.Id);
        Assert.Equal(profile.Name, retrievedProfile.Name);
        Assert.Equal(profile.SystemPrompt, retrievedProfile.SystemPrompt);
        Assert.Equal(profile.TranscriptionModelId, retrievedProfile.TranscriptionModelId);
        Assert.Equal(profile.ChatModelId, retrievedProfile.ChatModelId);
    }

    [Fact]
    public void GetAll_Returns_All_Profiles()
    {
        // Arrange
        var profile1 = new Profile(new("Profile1"), new("SystemPrompt1"), new("openai-whisper-1"), new("openai-gpt-4o-mini"));
        var profile2 = new Profile(new("Profile2"), new("SystemPrompt2"), new("openai-whisper-1"), new("openai-gpt-4o-mini"));
        _repository.Add(profile1);
        _repository.Add(profile2);
        // Act
        var allProfiles = _repository.GetAll();
        // Assert
        Assert.Contains(allProfiles, p => p.Id == profile1.Id);
        Assert.Contains(allProfiles, p => p.Id == profile2.Id);
    }

    [Fact]
    public void Update_And_Get_Works()
    {
        // Arrange
        var profile = new Profile(new("TestProfile"), new("TestSystemPrompt\n\n\na"), new("openai-whisper-1"), new("openai-gpt-4o-mini"));
        _repository.Add(profile);
        profile = profile with { Name = new("UpdatedProfile") };
        // Act
        _repository.Update(profile);
        var updatedProfile = _repository.Get(profile.Id);
        // Assert
        Assert.NotNull(updatedProfile);
        Assert.Equal(profile.Name, updatedProfile.Name);
    }

    [Fact]
    public void Delete_And_Get_Returns_Null()
    {
        // Arrange
        var profile = new Profile(new("TestProfile"), new("TestSystemPrompt\n\n\na"), new("openai-whisper-1"), new("openai-gpt-4o-mini"));
        _repository.Add(profile);
        // Act
        _repository.Delete(profile.Id);
        var deletedProfile = _repository.Get(profile.Id);
        // Assert
        Assert.Null(deletedProfile);
    }
}