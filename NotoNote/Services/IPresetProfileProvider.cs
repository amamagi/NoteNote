using NotoNote.Models;

namespace NotoNote.Services;
public interface IPresetProfileProvider
{
    public List<Profile> Get();
}
