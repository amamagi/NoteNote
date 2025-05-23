namespace NotoNote.Models;
public interface IApiMetadataProvider
{
    public ApiMetadata Get(ApiSource source);
}
