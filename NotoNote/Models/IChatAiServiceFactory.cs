namespace NotoNote.Models;

public interface IChatAiServiceFactory
{
    IChatAiService Create(IChatAiModel model);
}
