namespace NotoNote.Models;

public record Profile(
    ProfileId Id,
    ProfileName Name,
    SystemPrompt SystemPrompt,
    TranscriptionModelId TranscriptionModelId,
    ChatModelId ChatModelId) : RecordWithValidation
{
    public Profile(
        ProfileName name,
        SystemPrompt systemPrompt,
        TranscriptionModelId transcriptionModel,
        ChatModelId chatModel) : this(
            new(Guid.NewGuid()),
            name,
            systemPrompt,
            transcriptionModel,
            chatModel)
    { }

    public static Profile Default => new Profile(
        new("New Profile"),
        new("あなたは音声の書き起こしを整形するアシスタントです。以下のルールに従って書き起こしを整形してください。\n- \n---"),
        Constants.AvailableTranscriptionModels[0].Id,
        Constants.AvailableChatModels[0].Id);
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