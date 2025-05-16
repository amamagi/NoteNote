namespace NotoNote.Models;
public interface IProfileRepository
{
    void AddOrUpdate(Profile profile);
    void Delete(Guid id);
    void MoveIndex(Guid id, int count);
    Profile? Get(Guid id);
    List<Profile> GetAll();
}
