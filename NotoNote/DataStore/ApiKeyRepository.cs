using LiteDB;
using NotoNote.Models;

namespace NotoNote.DataStore;
public sealed class ApiKeyRepository : IApiKeyRepository
{
    private readonly ILiteCollection<ApiKeyDto> _collection;

    public ApiKeyRepository(ILiteDbContext context)
    {
        _collection = context.ApiKeys;
    }

    public void Delete(ApiProvider apiProvider)
    {
        var bsonId = apiProvider.ToString();
        _collection.Delete(bsonId);
    }

    public ApiKey? Get(ApiProvider apiProvider)
    {
        var dto = _collection.FindById(apiProvider.ToString());
        if (dto == null) return null;
        if (dto.Value == null)
        {
            // Invalid record
            Delete(apiProvider);
            return null;
        }

        return dto.ToModel();
    }

    public IEnumerable<ApiKey> GetAll()
    {
        return _collection.FindAll()
            .OrderBy(x => x.Provider)
            .Select(x => x.ToModel());
    }

    public void Set(ApiKey apiKey)
    {
        _collection.Insert(apiKey.ToDto());
    }

    public void Update(ApiKey apiKey)
    {
        _collection.Update(apiKey.ToDto());
    }
}