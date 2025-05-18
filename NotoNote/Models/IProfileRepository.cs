namespace NotoNote.Models;
public interface IProfileRepository
{
    void AddOrUpdate(Profile profile);
    bool Delete(ProfileId id);
    void MoveIndex(ProfileId id, int count);
    Profile? Get(ProfileId id);
    List<Profile> GetAll();
}
