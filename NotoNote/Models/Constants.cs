namespace NotoNote.Models;

public static class Constants
{
    public static readonly IReadOnlyList<ITranscriptionModel> AvailableTranscriptionModels =
        [
            new OpenAiTranscribeAiModel(
                new TranscriptionModelId("openai-whisper-1"),
                new ModelName("Whisper"),
                new OpenAiApiId("whisper-1"))
        ];

    public static readonly IReadOnlyList<IChatModel> AvailableChatModels =
        [
            new OpenAiChatAiModel(
                new ChatModelId("openai-gpt-4o-mini"),
                new ModelName("ChatGPT 4o-mini"),
                new OpenAiApiId("gpt-4o-mini"))
        ];

    public static readonly Dictionary<TranscriptionModelId, ITranscriptionModel> TranscriptionModelMap
        = AvailableTranscriptionModels.ToDictionary(model => model.Id, model => model);

    public static readonly Dictionary<ChatModelId, IChatModel> ChatModelMap
        = AvailableChatModels.ToDictionary(model => model.Id, model => model);
}
