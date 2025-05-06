namespace NotoNote.Models;
public interface IChatAiService
{
    Task<ChatResponceText> CompleteChatAsync(SystemPrompt systemPrompt, TranscriptText transcript);
}
