namespace NotoNote.Models;

public static class Constants
{
    public static readonly OpenAiCompatibleTranscriptionModel DefaultTranscriptionModelId = new(new("Whisper"), new("whisper-1"), ApiSource.OpenAI);
    public static readonly OpenAiCompatibleChatModel DefaultChatModelId = new(new("ChatGPT-4o mini"), new("gpt-4o-mini"), ApiSource.OpenAI);
}
