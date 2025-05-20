
namespace NotoNote.Models;
public record ApiSource(string Name)
{
    public static readonly ApiSource OpenAI = new("OpenAI");
    public static readonly ApiSource Gemini = new("Gemini");
    public static readonly ApiSource Anthropic = new("Anthropic");
    public override string ToString() => Name;
}

public record ApiSourceWithUrl(ApiSource ApiSource, Uri Uri)
{
    public override string ToString()
    {
        return this.ApiSource.ToString();
    }
}
