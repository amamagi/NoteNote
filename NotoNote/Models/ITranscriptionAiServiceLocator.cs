namespace NotoNote.Models;

public interface ITranscriptionAiServiceLocator
{
    ITranscriptionAiService GetService(ITranscriptionAiModel model);
}
