namespace TokenFight.Core.Enums.Attrs;

/// <summary>
/// 属性枚举
/// </summary>
public enum AttrType
{
    /// <summary> 生命值 </summary>
    Health,
    /// <summary> 攻击力 </summary>
    Attack,
    /// <summary> 防御力 </summary>
    Defense,
    /// <summary> 速度 </summary>
    Speed,
    /// <summary> 能量上限 </summary>
    MaxEnergy,
    /// <summary> 暴击率 </summary>
    CriticalRate,
    /// <summary> 暴击伤害 </summary>
    CriticalDamage,

    /// <summary> 施加的伤害提升 </summary>
    DamageIncrease,
    /// <summary> 受到的伤害减少 </summary>
    DamageReduction,
    /// <summary> 受到的伤害提升 | 易伤 </summary>
    Vulnerability,
    /// <summary> 穿透 </summary>
    DamagePenetrate,
    /// <summary> 抗性 </summary>
    DamageResistance,
    /// <summary> 减防 </summary>
    DefenseReduce,

    /// <summary> 提供的治疗提升 </summary>
    HealIncrease,
    /// <summary> 提供的护盾提升 </summary>
    ShieldIncrease,

    /// <summary> 结束标记 | 非属性 </summary>
    EndTag
}