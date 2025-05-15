namespace NotoNote.Models;
public interface IProfileRepository
{
    void AddOrUpdate(Profile profile);
    void Delete(Guid id);
    void Swap(Guid id1, Guid id2);
    Profile? Get(Guid id);
    List<Profile> GetAll();
}
