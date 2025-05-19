using LiteDB;
using NotoNote.Models;

namespace NotoNote.DataStore;
public sealed class ProfileDto
{
    [BsonId]
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string SystemPrompt { get; set; } = string.Empty;
    public string TranscriptionAiModelId { get; set; } = string.Empty;
    public string ChatModelId { get; set; } = string.Empty;
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
            new TranscriptionModelId(dto.TranscriptionAiModelId),
            new ChatModelId(dto.ChatModelId));
    }
    public static ProfileDto ToDto(this Profile model, Guid nextId)
    {
        return new ProfileDto
        {
            Id = model.Id.Value,
            Name = model.Name.Value,
            SystemPrompt = model.SystemPrompt.Value,
            TranscriptionAiModelId = model.TranscriptionModelId.Value,
            ChatModelId = model.ChatModelId.Value,
            NextId = nextId
        };
    }
}