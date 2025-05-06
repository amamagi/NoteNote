namespace NotoNote.Models;

public record OpenAiTranscribeAiModel(TranscriptionAiModelId Id, string ApiId, string DisplayName)
    : RecordWithValidation, ITranscriptionAiModel
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(DisplayName)) throw new ArgumentException();
        if (string.IsNullOrEmpty(ApiId)) throw new ArgumentException();
    }
}
public record OpenAiChatAiModel(ChatAiModelId Id, string ApiId, string DisplayName)
    : RecordWithValidation, IChatAiModel
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(DisplayName)) throw new ArgumentException();
        if (string.IsNullOrEmpty(ApiId)) throw new ArgumentException();
    }
}
