namespace NotoNote.Models;

public static class Constants
{
    public static readonly IReadOnlyList<ITranscriptionAiModel> AvailableTranscriptionAiModels =
        [
            new OpenAiTranscribeAiModel(
                new TranscriptionAiModelId("openai-whisper-1"),
                new AiModelName("Whisper"),
                new OpenAiApiId("whisper-1"))
        ];

    public static readonly IReadOnlyList<IChatAiModel> AvailableChatAiModels =
        [
            new OpenAiChatAiModel(
                new ChatAiModelId("openai-gpt-4o-mini"),
                new AiModelName("ChatGPT 4o-mini"),
                new OpenAiApiId("gpt-4o-mini"))
        ];

    public static readonly Dictionary<TranscriptionAiModelId, ITranscriptionAiModel> TranscriptionAiModelMap
        = AvailableTranscriptionAiModels.ToDictionary(model => model.Id, model => model);

    public static readonly Dictionary<ChatAiModelId, IChatAiModel> ChatAiModelMap
        = AvailableChatAiModels.ToDictionary(model => model.Id, model => model);
}
