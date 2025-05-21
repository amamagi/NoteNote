namespace NotoNote.Models;
public interface IApiKeyRepository
{
    public ApiKey? Get(ApiSource apiProvider);
    public ApiKey? GetFromSaveData(ApiSource apiProvider);
    public ApiKey? GetFromPreset(ApiSource apiProvider);
    public void AddOrUpdate(ApiKey apiKey);
    void Delete(ApiSource apiProvider);
}