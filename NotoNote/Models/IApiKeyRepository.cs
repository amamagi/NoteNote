namespace NotoNote.Models;
public interface IApiKeyRepository
{
    public IEnumerable<ApiKey> GetAll();
    public ApiKey? Get(ApiProvider apiProvider);
    public void AddOrUpdate(ApiKey apiKey);
    void Delete(ApiProvider apiProvider);
}