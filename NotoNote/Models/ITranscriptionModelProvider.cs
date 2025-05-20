namespace NotoNote.Models;
public interface ITranscriptionModelProvider
{
    public ITranscriptionModel? Get(TranscriptionModelId id);
    public IEnumerable<ITranscriptionModel> GetAll();
}
