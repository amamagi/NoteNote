namespace NotoNote.Models;
public interface IProfileRepository
{
    void Add(Profile profile);
    Profile? Get(ProfileId id);
    IEnumerable<Profile> GetAll();
    void Update(Profile profile);
    void Delete(ProfileId id);
}
