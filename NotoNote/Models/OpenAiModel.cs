using NotoNote.Utilities;

namespace NotoNote.Models;

public record OpenAiTranscribeAiModel(TranscriptionModelId Id, ModelName DisplayName, OpenAiApiId ApiId)
    : RecordWithValidation, ITranscriptionModel;

public record OpenAiChatAiModel(ChatModelId Id, ModelName DisplayName, OpenAiApiId ApiId)
    : RecordWithValidation, IChatModel;

public record OpenAiApiId(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}