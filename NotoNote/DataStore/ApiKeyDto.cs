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
            Provider = apiKey.Source.ToString(),
            Value = apiKey.Value
        };
    }

    public static ApiKey ToModel(this ApiKeyDto dto)
    {
        return new ApiKey(new(dto.Provider), dto.Value);
    }

}