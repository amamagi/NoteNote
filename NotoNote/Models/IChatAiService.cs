namespace NotoNote.Services;
public interface IChatAiService
{
    Task<string> CompleteChatAsync(string systemPrompt, string transcript);
}
