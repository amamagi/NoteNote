namespace NotoNote.Models;

internal static class StringExtensions
{
    public static void ThrowIfNullOrEmpty(this string value, string message = "")
    {
        if (string.IsNullOrEmpty(value)) throw new ArgumentException(message);
    }
}
