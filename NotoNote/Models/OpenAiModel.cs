namespace NotoNote.Models;

public record OpenAiTranscribeAiModel(TranscriptionAiModelId Id, AiModelName DisplayName, OpenAiApiId ApiId)
    : RecordWithValidation, ITranscriptionAiModel;
public record OpenAiChatAiModel(ChatAiModelId Id, AiModelName DisplayName, OpenAiApiId ApiId)
    : RecordWithValidation, IChatAiModel;

public record OpenAiApiId(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}