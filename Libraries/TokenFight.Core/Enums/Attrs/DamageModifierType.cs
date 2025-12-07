namespace TokenFight.Core.Enums.Attrs;

/// <summary>
/// 伤害修饰类型（用于与 Element 或 DamageType 组合生成唯一整型 ID）
/// </summary>
public enum DamageModifierType
{
    /// <summary>
    /// 增伤 - 提升自身造成的元素或属性伤害
    /// </summary>
    DamageIncrease = 10000,

    /// <summary>
    /// 减伤 - 降低自身受到的元素或属性伤害
    /// </summary>
    DamageReduction = 20000,

    /// <summary>
    /// 易伤 - 使目标受到的元素或属性伤害增加
    /// </summary>
    Vulnerability = 30000,

    /// <summary>
    /// 穿透 - 忽略目标的部分元素或属性抗性
    /// </summary>
    Penetration = 40000,

    /// <summary>
    /// 抗性 - 提升自身对元素或属性伤害的抵抗能力
    /// </summary>
    Resistance = 50000,
}