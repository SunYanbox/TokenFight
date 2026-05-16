namespace TokenFight.Core.Enums.Attrs;

public enum DamageType
{
    /// <summary>普攻伤害</summary>
    NormalAttack = AttrType.EndTag + 1,

    /// <summary>追加伤害</summary>
    AdditionalAttack,

    /// <summary>终结技伤害</summary>
    UltimateSkill,

    /// <summary>追击伤害</summary>
    PursuitAttack,

    /// <summary>战技伤害</summary>
    BattleSkill,

    /// <summary>附加伤害</summary>
    AttachedDamage,

    /// <summary>真实伤害</summary>
    RealDamage,

    /// <summary>持续伤害</summary>
    DotDamage,

    /// <summary>暴击伤害</summary>
    CriticalDamage,

    /// <summary> 结束标记 | 非属性 </summary>
    EndTag
}
