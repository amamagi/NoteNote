namespace NotoNote.Models;

public interface IChatServiceFactory
{
    IChatService Create(IChatModel model);
}
