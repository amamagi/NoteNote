namespace NotoNote.Models;
public interface IChatAiService
{
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="TaskCanceledException"></exception>
    Task<ChatResponceText> CompleteChatAsync(SystemPrompt systemPrompt, TranscriptText transcript, CancellationToken ct = default);
}
