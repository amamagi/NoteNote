namespace NotoNote.DataStore;
public sealed class CollapsedDatabaseException : Exception
{
    public CollapsedDatabaseException(string message) : base(message) { }
}