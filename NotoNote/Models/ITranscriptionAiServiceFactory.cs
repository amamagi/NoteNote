namespace NotoNote.Models;

public interface ITranscriptionAiServiceFactory
{
    ITranscriptionAiService Create(ITranscriptionAiModel model);
}
