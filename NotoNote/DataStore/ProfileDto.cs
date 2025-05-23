using LiteDB;
using NotoNote.Models;

namespace NotoNote.DataStore;
public sealed class ProfileDto
{
    [BsonId]
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string SystemPrompt { get; set; } = string.Empty;
    public ITranscriptionModel TranscriptionModelId { get; set; } = Constants.DefaultTranscriptionModelId;
    public IChatModel ChatModelId { get; set; } = Constants.DefaultChatModelId;
    public Guid NextId { get; set; }
}

public static class ProfileExtensions
{
    public static Profile ToModel(this ProfileDto dto)
    {
        return new Profile(
            new ProfileId(dto.Id),
            new ProfileName(dto.Name),
            new SystemPrompt(dto.SystemPrompt),
            dto.TranscriptionModelId,
            dto.ChatModelId);
    }
    public static ProfileDto ToDto(this Profile model, Guid nextId)
    {
        return new ProfileDto
        {
            Id = model.Id.Value,
            Name = model.Name.Value,
            SystemPrompt = model.SystemPrompt.Value,
            TranscriptionModelId = model.TranscriptionModelId,
            ChatModelId = model.ChatModelId,
            NextId = nextId
        };
    }
}