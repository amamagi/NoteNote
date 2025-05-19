using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace NotoNote.Utilities;
public class ObservableCollection2<T>(IEnumerable<T> collection) : ObservableCollection<T>(collection)
{
    public void AddRange(IEnumerable<T> addItems)
    {
        if (addItems == null)
        {
            throw new ArgumentNullException(nameof(addItems));
        }

        foreach (var item in addItems)
        {
            Items.Add(item);
        }

        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
