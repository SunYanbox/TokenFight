namespace TokenFight.Core.Interfaces.Attrs;

/// <summary>
/// 属性字典泛型 内部储存方式为 int(type) -> { Id -> 值 }
///
/// </summary>
public interface IDictManager
{
    /// <summary>
    /// 初始化DictManager类, 设置可用的类型(类型在内部储存为整数)
    /// 会清理已有的数据
    /// </summary>
    /// <param name="ids"></param>
    public void Init(HashSet<int> ids);

    /// <summary> 覆盖式添加新的属性 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="id">数据的Id</param>
    /// <param name="value">数据的值</param>
    public bool Add(int type, string id, double value);

    /// <summary> 通过属性类型和Id移除数据 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="id">数据的Id</param>
    public bool Remove(int type, string id);

    /// <summary> 移除来自Id的所有属性下的数据 </summary>
    /// <param name="id">数据的Id</param>
    public void RemoveByKey(string id);

    /// <summary> 清理所有数据 </summary>
    public void Clear();

    /// <summary> 在保留属性的情况下, 清理每个属性下面的所有数据 </summary>
    public void ClearValues();

    /// <summary> 获取指定属性下是否包含指定Id的数据 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="id">数据的Id</param>
    /// <returns></returns>
    public bool ContainsKey(int type, string id);

    /// <summary> 获取指定属性下指定Id的数据 </summary>
    /// <param name="type">属性类型索引</param>
    /// <param name="id">数据的Id</param>
    /// <returns></returns>
    public double GetValue(int type, string id);

    /// <summary> 获取指定属性的数据和 </summary>
    /// <param name="type">属性类型索引</param>
    /// <returns></returns>
    public IEnumerable<double> GetValues(int type);

    /// <summary> 尝试获取指定属性下指定Id的数据 </summary>
    /// <param name="type"></param>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns>成功获取返回true</returns>
    public bool TryGet(int type, string id, out double? value);

    /// <summary> 获取属性数量 </summary>
    public int Count { get; }

    public string ToDetailString(HashSet<object>? visited = null);
}
