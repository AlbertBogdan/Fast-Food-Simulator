using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Events;

public class ObservableList<T> : List<T>
{
    public event EventHandler<ItemAddedEventArgs<T>> ItemAdded;

    public string ListName { get; }

    public ObservableList(string listName)
    {
        ListName = listName;
    }

    public new void Add(T item)
    {
        base.Add(item);
        OnItemAdded(item);
    }

    protected virtual void OnItemAdded(T item)
    {
        ItemAdded?.Invoke(this, new ItemAddedEventArgs<T>(item));
    }
}

public class ItemAddedEventArgs<T> : EventArgs
{
    public T AddedItem { get; }

    public ItemAddedEventArgs(T item)
    {
        AddedItem = item;
    }
}