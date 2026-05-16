namespace TokenFight.Core.Interfaces.Bases;

/// <summary>
/// 表示一个可变的枚举类型集合，支持增、删、查与遍历。
/// </summary>
/// <typeparam name="T">必须是 struct 且为 Enum 类型</typeparam>
public interface IEnumSet<T> : IEnumerable<T>
    where T : struct, Enum
{
    // ———————— 增 ————————
    /// <summary>
    /// 添加一个枚举值。
    /// </summary>
    /// <param name="type">要添加的枚举值</param>
    /// <returns>若成功添加（之前不存在），返回 true；否则 false</returns>
    bool Add(T type);

    /// <summary>
    /// 添加多个枚举值。
    /// </summary>
    /// <param name="types">要添加的枚举值数组</param>
    void AddRange(params T[] types);

    // ———————— 删 ————————
    /// <summary>
    /// 移除一个枚举值。
    /// </summary>
    /// <param name="type">要移除的枚举值</param>
    /// <returns>若成功移除（之前存在），返回 true；否则 false</returns>
    bool Remove(T type);

    /// <summary>
    /// 清空所有枚举值。
    /// </summary>
    void Clear();

    // ———————— 查 ————————
    /// <summary>
    /// 判断是否包含指定枚举值。
    /// </summary>
    /// <param name="type">要检查的枚举值</param>
    /// <returns>若包含，返回 true；否则 false</returns>
    bool Contains(T type);

    /// <summary>
    /// 判断是否包含任意一个指定的枚举值。
    /// </summary>
    /// <param name="types">要检查的枚举值数组</param>
    /// <returns>若包含其中任意一个，返回 true；否则 false</returns>
    bool ContainsAny(params T[] types);

    /// <summary>
    /// 判断是否包含所有指定的枚举值。
    /// </summary>
    /// <param name="types">要检查的枚举值数组</param>
    /// <returns>若全部包含，返回 true；否则 false</returns>
    bool ContainsAll(params T[] types);

    /// <summary>
    /// 获取集合中枚举值的数量。
    /// </summary>
    int Count { get; }

    /// <summary>
    /// 判断集合是否为空。
    /// </summary>
    bool IsEmpty { get; }
}
