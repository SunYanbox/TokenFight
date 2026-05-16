namespace TokenFight.Core.Interfaces.Entities.Masters;

/// <summary>
/// 护盾系统
///
/// 最终护盾量=Max(所有护盾的值)
///
/// 受到攻击时: 同步削减所有护盾, 并移除被破除的护盾
/// </summary>
public interface IShieldMaster : IMaster
{
    /// <summary> 覆盖形式添加护盾 </summary>
    /// <param name="id">护盾Id</param>
    /// <param name="value">护盾值</param>
    public void Add(string id, double value);

    /// <summary>
    /// 移除指定Id的护盾
    ///
    /// 如果Id为null, 则清空所有护盾
    /// </summary>
    /// <param name="id">护盾Id</param>
    public void Remove(string? id);

    /// <summary> 移除多个键的护盾 </summary>
    /// <param name="ids">护盾Id的可枚举迭代器</param>
    public void RemoveShields(IEnumerable<string> ids);

    /// <summary> 获取护盾值 </summary>
    public double Shield { get; }

    /// <summary> 造成伤害并获取剩余伤害与护盾抵消的伤害值 </summary>
    /// <remarks> 这个函数内部不应该发布事件 </remarks>
    /// <param name="damage">需要造成的伤害</param>
    /// <param name="residualDamage">剩余伤害</param>
    /// <param name="shieldDefense">护盾抵消的伤害值</param>
    public void TakeDamage(double damage, out double residualDamage, out double shieldDefense);

    /// <summary> 获取指定Id的护盾值 </summary>
    public double GetShield(string id);
}