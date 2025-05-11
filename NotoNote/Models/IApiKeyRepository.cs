namespace NotoNote.Models;
public interface IApiKeyRepository
{
    public IEnumerable<ApiKey> GetAll();
    public ApiKey? Get(ApiProvider apiProvider);
    public void Set(ApiKey apiKey);
    public void Update(ApiKey apiKey);
    void Delete(ApiProvider apiProvider);
}