using NotoNote.Models;

namespace NotoNote.DataStore;
public sealed class ProfileDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SystemPrompt { get; set; } = string.Empty;
    public string TranscriptionAiModelId { get; set; } = string.Empty;
    public string ChatModelId { get; set; } = string.Empty;
    public int Order { get; set; } = 0;
}

public static class ProfileExtensions
{
    public static Profile ToModel(this ProfileDto dto)
    {
        return new Profile(
            new ProfileId(dto.Id),
            new ProfileName(dto.Name),
            new SystemPrompt(dto.SystemPrompt),
            new TranscriptionAiModelId(dto.TranscriptionAiModelId),
            new ChatAiModelId(dto.ChatModelId));
    }
    public static ProfileDto ToDto(this Profile model, int order = int.MaxValue)
    {
        return new ProfileDto
        {
            Id = model.Id.Value,
            Name = model.Name.Value,
            SystemPrompt = model.SystemPrompt.Value,
            TranscriptionAiModelId = model.TranscriptionModelId.Value,
            ChatModelId = model.ChatModelId.Value,
            Order = order
        };
    }
}