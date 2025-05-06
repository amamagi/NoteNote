using NotoNote.Models;

namespace NotoNote.Services;
public interface IChatAiService
{
    Task<ChatResponceText> CompleteChatAsync(SystemPrompt systemPrompt, TranscriptText transcript);
}
