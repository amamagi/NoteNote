using NotoNote.Utilities;

namespace NotoNote.Models;

public record OpenAiCompatibleTranscriptionModel(ModelName DisplayName, ApiModelId ApiId, ApiSource ApiSource) : ITranscriptionModel
{
}

public record OpenAiCompatibleChatModel(ModelName DisplayName, ApiModelId ApiId, ApiSource ApiSource) : IChatModel
{
}