using NotoNote.Services;

namespace NotoNote.Models;

public interface IChatAiServiceLocator
{
    IChatAiService GetService(IChatAiModel model);
}
