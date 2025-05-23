
namespace NotoNote.Models;
public interface IChatModelProvider
{
    public IEnumerable<IChatModel> GetAll();
}
