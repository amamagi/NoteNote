using NotoNote.Utilities;

namespace NotoNote.Models;

public record OpenAiCompatibleTranscribeModel(
    TranscriptionModelId Id,
    ModelName DisplayName,
    ApiSourceWithUrl ApiSource,
    ApiModelId ApiId,
    bool RequireApiKey)
    : RecordWithValidation, ITranscriptionModel;

public record OpenAiCompatibleChatModel(
    ChatModelId Id,
    ModelName DisplayName,
    ApiSourceWithUrl ApiSource,
    ApiModelId ApiId,
    bool RequireApiKey)
    : RecordWithValidation, IChatModel;

public record ApiModelId(string Value) : RecordWithValidation
{
    protected override void Validate()
    {
        Value.ThrowIfNullOrEmpty();
    }
}