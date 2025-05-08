namespace NotoNote.Models;

public record Profile(
    ProfileId Id,
    ProfileName Name,
    SystemPrompt SystemPrompt,
    TranscriptionAiModelId TranscriptionModelId,
    ChatAiModelId ChatModelId) : RecordWithValidation
{
    public Profile(
        ProfileName name,
        SystemPrompt systemPrompt,
        TranscriptionAiModelId transcriptionModel,
        ChatAiModelId chatModel) : this(
            new(Guid.NewGuid()),
            name,
            systemPrompt,
            transcriptionModel,
            chatModel)
    { }
}

public record ProfileId(Guid Value) : RecordWithValidation
{
    protected override void Validate()
    {
        if (Value == Guid.Empty)
            throw new ArgumentException("Profile ID cannot be empty.");
    }
}

public record ProfileName(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}