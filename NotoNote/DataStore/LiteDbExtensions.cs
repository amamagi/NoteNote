using LiteDB;

namespace NotoNote.DataStore;
public static class LiteDbExtensions
{
    public static void AddOrUpdate<T>(this ILiteCollection<T> collection, T value, BsonValue id)
    {
        if (collection.FindById(id) == null)
        {
            collection.Insert(value);
        }
        else
        {
            collection.Update(value);
        }
    }
}