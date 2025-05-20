namespace NotoNote.Models;
public interface IApiKeyProvider
{
    public string GetApiKey(ApiSource source);
}
