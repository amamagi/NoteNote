namespace NotoNote.Models;
public interface IProfileRepository
{
    ProfileId GetActiveProfileId();
    bool SetActiveProfile(ProfileId id);
    void AddOrUpdate(Profile profile);
    bool Delete(ProfileId id);
    void MoveIndex(ProfileId id, int count);
    Profile? Get(ProfileId id);
    List<Profile> GetAll();
}
