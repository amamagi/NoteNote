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

    public static Profile Default => new Profile(
        new("Default"),
        new("あなたは音声の書き起こしを整形するアシスタントです。以下の書き起こしを整形してください\n---"),
        Constants.AvailableTranscriptionAiModels[0].Id,
        Constants.AvailableChatAiModels[0].Id);
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
    }
}