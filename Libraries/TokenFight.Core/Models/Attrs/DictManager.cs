using System.Text;
using TokenFight.Core.Interfaces.Attrs;

namespace TokenFight.Core.Models.Attrs;

/// <summary>
/// 属性字典泛型 内部储存方式为 int(type) -> { Id -> 值 }
///
/// </summary>
public class DictManager : IDictManager
{
    private readonly Dictionary<int, Dictionary<string, double>> _dict = new();
    private readonly Dictionary<string, HashSet<int>> _added = new();

    /// <summary>
    /// 初始化DictManager类, 设置可用的类型(类型在内部储存为整数)
    /// 会清理已有的数据
    /// </summary>
    /// <param name="ids"></param>
    public void Init(HashSet<int> ids)
    {
        Clear();
        foreach (int k in ids)
        {
            _dict.Add(k, new Dictionary<string, double>());
        }
    }

    /// <summary> 覆盖式添加新的属性 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="id">数据的Id</param>
    /// <param name="value">数据的值</param>
    public bool Add(int type, string id, double value)
    {
        if (!_dict.TryGetValue(type, out Dictionary<string, double>? value1)) return false;
        value1.Remove(id);
        if (!_dict[type].TryAdd(id, value)) return false;
        if (!_added.ContainsKey(id)) _added.Add(id, []);
        _added[id].Add(type);
        return true;
    }

    /// <summary> 通过属性类型和Id移除数据 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="id">数据的Id</param>
    public bool Remove(int type, string id)
    {
        if (!_dict.TryGetValue(type, out Dictionary<string, double>? attr)) return false;
        if (!attr.Remove(id)) return false;
        if (_added.TryGetValue(id, out HashSet<int>? value))
        {
            value.Remove(type);
        }
        return true;
    }

    /// <summary> 移除来自Id的所有属性下的数据 </summary>
    /// <param name="id">数据的Id</param>
    public void RemoveByKey(string id)
    {
        if (!_added.TryGetValue(id, out HashSet<int>? value)) return;
        foreach (int k in value)
        {
            _dict[k].Remove(id);
        }
        _added[id].Clear();
    }

    /// <summary> 清理所有数据 </summary>
    public void Clear()
    {
        _dict.Clear();
        _added.Clear();
    }

    /// <summary> 在保留属性的情况下, 清理每个属性下面的所有数据 </summary>
    public void ClearValues()
    {
        foreach (Dictionary<string, double> v in _dict.Values)
        {
            v.Clear();
        }
        _added.Clear();
    }

    /// <summary> 获取指定属性下是否包含指定Id的数据 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="id">数据的Id</param>
    /// <returns></returns>
    public bool ContainsKey(int type, string id) => _dict.ContainsKey(type) && _dict[type].ContainsKey(id);

    /// <summary> 获取指定属性下指定Id的数据 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="id">数据的Id</param>
    /// <returns></returns>
    public double GetValue(int type, string id) => _dict[type][id];

    /// <summary> 获取指定属性的数据和 </summary>
    /// <param name="type">属性类型索引</param>
    /// <returns></returns>
    public IEnumerable<double> GetValues(int type) => _dict[type].Values.ToArray();

    /// <summary> 尝试获取指定属性下指定Id的数据 </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns>成功获取返回true</returns>
    public bool TryGet(int type, string id, out double? value)
    {
        value = null;
        if (!_dict.TryGetValue(type, out Dictionary<string, double>? valueI)) return false;
        if (!valueI.TryGetValue(id, out double valueJ)) return false;
        value = valueJ;
        return true;
    }

    /// <summary> 获取属性数量 </summary>
    public int Count => _dict.Count;

    public string ToDetailString(HashSet<object>? visited = null)
    {
        visited ??= [];
        visited.Add(this);
        var propertiesBuilder = new StringBuilder();
        propertiesBuilder.Append("{ ");
        foreach ((int name, Dictionary<string, double> value) in _dict)
        {
            propertiesBuilder.Append($"{name}: {value}");
            propertiesBuilder.Append(", ");
        }
        propertiesBuilder.Append(" }");
        return $"{GetType().Name} {propertiesBuilder}";
    }
}
