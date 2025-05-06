namespace NotoNote.Models;
public static class AiModelConsts
{
    public static IReadOnlyList<IChatAiModel> ChatAiModels =
    [
        new OpenAiChatAiModel(new("openai-gpt-4o-mini"), "GPT 4o mini", "gpt-4o-mini")
    ];

    public static IReadOnlyList<ITranscriptionAiModel> TranscriptionAiModels =
        [
            new OpenAiTranscribeAiModel(new("openai-whisper-1"), "Whisper", "whisper-1")
        ];
}
