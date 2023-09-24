using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastFoodSimulatorLibrary.Events;

public class ObservableQueue<T> : Queue<T>
{
    public event EventHandler<ItemEnqueuedEventArgs<T>> ItemEnqueued;
    public event EventHandler<ItemDequeuedEventArgs<T>> ItemDequeued;

    public string QueueName { get; }

    public ObservableQueue(string queueName)
    {
        QueueName = queueName;
    }

    public new void Enqueue(T item)
    {
        base.Enqueue(item);
        OnItemChanged(item);
    }

    public new T Dequeue()
    {
        T item = base.Dequeue();
        OnItemChanged(item);
        return item;
    }

    protected virtual void OnItemChanged(T item)
    {
        if (base.Contains(item))
        {
            ItemEnqueued?.Invoke(this, new ItemEnqueuedEventArgs<T>(item));
        }
        else
        {
            ItemDequeued?.Invoke(this, new ItemDequeuedEventArgs<T>(item));
        }
    }
}

public class ItemEnqueuedEventArgs<T> : EventArgs
{
    public T EnqueuedItem { get; }

    public ItemEnqueuedEventArgs(T item)
    {
        EnqueuedItem = item;
    }
}

public class ItemDequeuedEventArgs<T> : EventArgs
{
    public T DequeuedItem { get; }

    public ItemDequeuedEventArgs(T item)
    {
        DequeuedItem = item;
    }
}