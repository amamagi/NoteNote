using Moq;
using NotoNote.DataStore;
using NotoNote.Models;
using NotoNote.Services;

namespace NotoNote.Tests;
public sealed class LiteDbProfileRepositoryTest : IDisposable
{
    private readonly string _tempDbPath;
    private readonly ProfileRepository _repository;
    private readonly ILiteDbContext _context;

    public LiteDbProfileRepositoryTest()
    {
        // Create a temporary database path
        _tempDbPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".db");
        _context = new LiteDbContext(_tempDbPath);
        var presets = new Mock<IPresetProfileProvider>();
        presets.Setup(x => x.Get()).Returns([]);
        _repository = new ProfileRepository(presets.Object, _context);
    }

    public void Dispose()
    {
        _context?.Dispose();
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
        var profile = new Profile(new("TestProfile"), new("TestSystemPrompt\n\n\na"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        // Act
        _repository.AddOrUpdate(profile);
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
    public void GetAll_Returns_All_Profiles_In_Added_Order()
    {
        // Arrange
        var profile1 = new Profile(new("Profile1"), new("SystemPrompt1"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        var profile2 = new Profile(new("Profile2"), new("SystemPrompt2"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        _repository.AddOrUpdate(profile1);
        _repository.AddOrUpdate(profile2);
        // Act
        var allProfiles = _repository.GetAll();
        // Assert
        // 末尾に追加される
        Assert.Equal(allProfiles[^2].Id, profile1.Id);
        Assert.Equal(allProfiles[^1].Id, profile2.Id);
    }

    [Fact]
    public void Update_And_Get_Works()
    {
        // Arrange
        var profile = new Profile(new("TestProfile"), new("TestSystemPrompt\n\n\na"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        _repository.AddOrUpdate(profile);
        profile = profile with { Name = new("UpdatedProfile") };
        // Act
        _repository.AddOrUpdate(profile);
        var updatedProfile = _repository.Get(profile.Id);
        // Assert
        Assert.NotNull(updatedProfile);
        Assert.Equal(profile.Name, updatedProfile.Name);
    }

    [Fact]
    public void Delete_And_Get_Returns_Null()
    {
        // Arrange
        var profile = new Profile(new("TestProfile"), new("TestSystemPrompt\n\n\na"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        _repository.AddOrUpdate(profile);
        // Act
        _repository.Delete(profile.Id);
        var deletedProfile = _repository.Get(profile.Id);
        // Assert
        Assert.Null(deletedProfile);
    }

    [Fact]
    public void Delete_Reconstruct_Linking()
    {
        // Arrange
        var profile1 = new Profile(new("Profile1"), new("SystemPrompt1"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        var profile2 = new Profile(new("Profile2"), new("SystemPrompt2"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        var profile3 = new Profile(new("Profile3"), new("SystemPrompt3"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        _repository.AddOrUpdate(profile1);
        _repository.AddOrUpdate(profile2);
        _repository.AddOrUpdate(profile3);

        // Act
        _repository.Delete(profile2.Id);
        var allProfiles = _repository.GetAll();

        // Assert
        Assert.Equal(allProfiles[^2].Id, profile1.Id);
        Assert.Equal(allProfiles[^1].Id, profile3.Id);
    }

    [Fact]
    public void Move_Index_Forward_Changes_Profile_Order()
    {
        // Arrange
        var profile1 = new Profile(new("Profile1"), new("SystemPrompt1"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        var profile2 = new Profile(new("Profile2"), new("SystemPrompt2"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        var profile3 = new Profile(new("Profile3"), new("SystemPrompt3"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        _repository.AddOrUpdate(profile1);
        _repository.AddOrUpdate(profile2);
        _repository.AddOrUpdate(profile3);
        // Act
        _repository.MoveIndex(profile1.Id, 2);
        var allProfiles = _repository.GetAll();
        // Assert
        Assert.Equal(allProfiles[^3].Id, profile2.Id);
        Assert.Equal(allProfiles[^2].Id, profile3.Id);
        Assert.Equal(allProfiles[^1].Id, profile1.Id);
    }


    [Fact]
    public void Move_Index_Backward_Changes_Profile_Order()
    {
        // Arrange
        var profile1 = new Profile(new("Profile1"), new("SystemPrompt1"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        var profile2 = new Profile(new("Profile2"), new("SystemPrompt2"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        var profile3 = new Profile(new("Profile3"), new("SystemPrompt3"), Constants.DefaultTranscriptionModelId, Constants.DefaultChatModelId);
        _repository.AddOrUpdate(profile1);
        _repository.AddOrUpdate(profile2);
        _repository.AddOrUpdate(profile3);
        // Act
        _repository.MoveIndex(profile3.Id, -2);
        var allProfiles = _repository.GetAll();
        // Assert
        Assert.Equal(allProfiles[^3].Id, profile3.Id);
        Assert.Equal(allProfiles[^2].Id, profile1.Id);
        Assert.Equal(allProfiles[^1].Id, profile2.Id);
    }
}