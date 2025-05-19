namespace NotoNote.Models;

public interface ITranscriptionServiceFactory
{
    ITranscriptionService Create(ITranscriptionModel model);
}
