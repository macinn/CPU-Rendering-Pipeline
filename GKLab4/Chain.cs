using System.Collections;

namespace Utils;
public class Chain<T> : ICollection<T>
{
    readonly List<ChainItem<T>> items;
    public Chain() { 
        items = [];
    }
    public Chain(List<T> items)
    {
        this.items = 
            items.Select((item, index) => new ChainItem<T>(item, index, this.items)).ToList();
    }
    public ChainItem<T> First { get { return items[0]; } }

    public int Count => throw new NotImplementedException();

    public bool IsReadOnly => throw new NotImplementedException();

    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        throw new NotImplementedException();
    }

    public bool Contains(T item)
    {
        throw new NotImplementedException();
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public bool Remove(T item)
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
public class ChainItem<T>
{
    readonly T obj;
    readonly int index;
    readonly List<ChainItem<T>>? list;
    internal ChainItem(T obj, int index, List<ChainItem<T>>? list)
    {
        this.obj = obj;
        this.index = index;
        this.list = list;
    }
    public ChainItem<T>? GetNext()
    {
        if (list == null || index == list.Count - 1)
            return null;

        return list[index + 1];
    }
    public T Get()
    {
        return obj;
    }
}
