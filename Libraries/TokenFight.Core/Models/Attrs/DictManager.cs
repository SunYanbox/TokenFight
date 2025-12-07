using System.Text;
using TokenFight.Core.Interfaces.Attrs;

namespace TokenFight.Core.Models.Attrs;

/// <summary>
/// 属性字典泛型 内部储存方式为 int(type) -> { Id -> 值 }
///
/// </summary>
public class DictManager: IDictManager
{
    private readonly Dictionary<int, Dictionary<string, double>> _dict = new();
    private readonly Dictionary<string, HashSet<int>> _added = new();
    
    /// <summary>
    /// 初始化DictManager类, 设置可用的类型(类型在内部储存为整数)
    /// 会清理已有的数据
    /// </summary>
    /// <param name="Ids"></param>
    public void Init(HashSet<int> Ids)
    {
        Clear();
        foreach (var k in Ids)
        {
            _dict.Add(k, new Dictionary<string, double>());
        }
    }

    /// <summary> 覆盖式添加新的属性 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="Id">数据的Id</param>
    /// <param name="value">数据的值</param>
    public bool Add(int type, string Id, double value)
    {
        if (!_dict.TryGetValue(type, out Dictionary<string, double>? value1)) return false;
        value1.Remove(Id);
        if (!_dict[type].TryAdd(Id, value)) return false;
        if (!_added.ContainsKey(Id)) _added.Add(Id, []);
        _added[Id].Add(type);
        return true;
    }

    /// <summary> 通过属性类型和Id移除数据 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="Id">数据的Id</param>
    public bool Remove(int type, string Id)
    {
        if (!_dict.TryGetValue(type, out Dictionary<string, double>? attr)) return false;
        if (!attr.Remove(Id)) return false;
        if (_added.TryGetValue(Id, out HashSet<int>? value))
        {
            value.Remove(type);
        }
        return true;
    }

    /// <summary> 移除来自Id的所有属性下的数据 </summary>
    /// <param name="Id">数据的Id</param>
    public void RemoveByKey(string Id)
    {
        if (!_added.TryGetValue(Id, out HashSet<int>? value)) return;
        foreach (var k in value)
        {
            _dict[k].Remove(Id);
        }
        _added[Id].Clear();
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
    /// <param name="Id">数据的Id</param>
    /// <returns></returns>
    public bool ContainsKey(int type, string Id) => _dict.ContainsKey(type) && _dict[type].ContainsKey(Id);
    
    /// <summary> 获取指定属性下指定Id的数据 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="Id">数据的Id</param>
    /// <returns></returns>
    public double GetValue(int type, string Id) => _dict[type][Id];

    /// <summary> 获取指定属性的数据和 </summary>
    /// <param name="type">属性类型索引</param>
    /// <returns></returns>
    public IEnumerable<double> GetValues(int type) => _dict[type].Values.ToArray();

    /// <summary> 尝试获取指定属性下指定Id的数据 </summary>
    /// <param name="type"></param>
    /// <param name="Id"></param>
    /// <param name="value"></param>
    /// <returns>成功获取返回true</returns>
    public bool TryGet(int type, string Id, out double? value)
    {
        value = null;
        if (!_dict.TryGetValue(type, out Dictionary<string, double>? valueI)) return false;
        if (!valueI.TryGetValue(Id, out double valueJ)) return false;
        value = valueJ;
        return true;
    }
    
    /// <summary> 获取属性数量 </summary>
    public int Count => _dict.Count;
    
    public string ToDetailString(HashSet<object>? visited = null)
    {
        visited ??= [];
        visited.Add(this);
        StringBuilder propertiesBuilder = new StringBuilder();
        propertiesBuilder.Append("{ ");
        foreach ((var name, Dictionary<string, double> value) in _dict)
        {
            propertiesBuilder.Append($"{name}: {value}");
            propertiesBuilder.Append(", ");
        }
        propertiesBuilder.Append(" }");
        return $"{GetType().Name} {propertiesBuilder}";
    }
}