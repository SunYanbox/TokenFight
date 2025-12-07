using System.Collections;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Entities.Masters;

public class EnumTypeMaster<T>: IEnumTypeMaster<T>
    where T : struct, Enum
{
    private readonly HashSet<T> _types = [];
    
    public EnumTypeMaster() { }

    public EnumTypeMaster(IEnumerable<T> types)
    {
        foreach (T type in types)
        {
            _types.Add(type);
        }
    }
    
    public IEnumerator<T> GetEnumerator()
    {
        return _types.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool Add(T type)
    {
        return _types.Add(type);
    }

    public void AddRange(params T[] types)
    {
        foreach (var type in types)
        {
            _types.Add(type);
        }
    }

    public bool Remove(T type)
    {
        return _types.Remove(type);
    }

    public void Clear()
    {
        _types.Clear();
    }

    public bool Contains(T type)
    {
        return _types.Contains(type);
    }

    public bool ContainsAny(params T[] types)
    {
        foreach (var type in types)
        {
            if (_types.Contains(type))
                return true;
        }
        return false;
    }

    public bool ContainsAll(params T[] types)
    {
        foreach (var type in types)
        {
            if (!_types.Contains(type))
                return false;
        }
        return true;
    }

    public int Count => _types.Count;
    public bool IsEmpty => _types.Count == 0;
}