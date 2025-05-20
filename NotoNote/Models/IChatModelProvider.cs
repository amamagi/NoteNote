
namespace NotoNote.Models;
public interface IChatModelProvider
{
    public IChatModel Get(ChatModelId id);
    public IEnumerable<IChatModel> GetAll();
}
