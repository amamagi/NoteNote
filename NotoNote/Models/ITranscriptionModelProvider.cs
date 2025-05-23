namespace NotoNote.Models;
public interface ITranscriptionModelProvider
{
    public IEnumerable<ITranscriptionModel> GetAll();
}
