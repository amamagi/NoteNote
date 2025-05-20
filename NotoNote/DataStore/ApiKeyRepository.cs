using LiteDB;
using Microsoft.Extensions.Options;
using NotoNote.Models;
using NotoNote.Services;

namespace NotoNote.DataStore;
public sealed class ApiKeyRepository : IApiKeyRepository
{
    private readonly ILiteCollection<ApiKeyDto> _collection;
    private readonly Dictionary<ApiSource, ApiKey> _apiKeysFromOption = new();

    public ApiKeyRepository(ILiteDbContext context, IOptions<List<OpenAiCompatibleApiOptions>> options)
    {
        _collection = context.ApiKeys;

        foreach (var option in options.Value)
        {
            if (string.IsNullOrEmpty(option.ApiKey) || string.IsNullOrEmpty(option.Name))
            {
                continue;
            }
            var source = new ApiSource(option.Name);
            var apiKey = new ApiKey(source, option.ApiKey);
            _apiKeysFromOption[source] = apiKey;
        }
    }

    public void Delete(ApiSource source)
    {
        var bsonId = source.ToString();
        _collection.Delete(bsonId);
    }

    public ApiKey? Get(ApiSource source)
    {
        var dto = _collection.FindById(source.ToString());
        if (dto == null)
        {
            if (_apiKeysFromOption.TryGetValue(source, out var apiKey))
            {
                return apiKey;
            }
            // No record found, return null
            return null;
        }
        if (dto.Value == null)
        {
            // Invalid record
            Delete(source);
            return null;
        }
        return dto.ToModel();
    }

    public void Set(ApiKey apiKey)
    {
        _collection.Insert(apiKey.ToDto());
    }

    public void AddOrUpdate(ApiKey apiKey)
    {
        var existing = Get(apiKey.Source);
        if (existing == null)
        {
            Set(apiKey);
            return;
        }
        _collection.Update(apiKey.ToDto());
    }
}