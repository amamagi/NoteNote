using NotoNote.Models;

namespace NotoNote.Services;
public static class PresetModelProvider
{
    public static readonly ITranscriptionModel DefaultTranscriptionModel = new OpenAiCompatibleTranscriptionModel(new("Whisper"), new("whisper-1"), ApiSource.OpenAI);
    public static readonly IChatModel DefaultChatModel = new OpenAiCompatibleChatModel(new("ChatGPT-4o mini"), new("gpt-4o-mini"), ApiSource.OpenAI);
    public static readonly ITranscriptionModel[] TranscriptionModels = [DefaultTranscriptionModel];
    public static readonly IChatModel[] ChatModels = [DefaultChatModel];
}