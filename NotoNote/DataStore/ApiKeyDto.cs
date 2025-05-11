using LiteDB;
using NotoNote.Models;

namespace NotoNote.DataStore;
public sealed class ApiKeyDto
{
    [BsonId]
    public string Provider { get; set; } = "";
    public string Value { get; set; } = "";
}

public static class ApiKeyExtensions
{
    public static ApiKeyDto ToDto(this ApiKey apiKey)
    {
        return new ApiKeyDto
        {
            Provider = apiKey.Provider.ToString(),
            Value = apiKey.Value
        };
    }

    public static ApiKey ToModel(this ApiKeyDto dto)
    {
        var provider = dto.Provider;
        if (!Enum.TryParse<ApiProvider>(provider, out var apiProvider)) throw new ArgumentException("Invalid ApiKeyProvider");
        if (!Enum.IsDefined(apiProvider)) throw new ArgumentException("Invalid ApiKeyProvider");
        return new ApiKey(apiProvider, dto.Value);
    }

}