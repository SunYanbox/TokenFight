namespace TokenFight.Core.Enums.Attrs;

/// <summary> 元素属性 </summary>
public enum Element
{
    /// <summary> 物理属性 </summary>
    Physics = DamageType.EndTag + 1,
    /// <summary> 冰属性 </summary>
    Ice,
    /// <summary> 量子属性 </summary>
    Quantum,
    /// <summary> 虚数属性 </summary>
    Imaginary,
    /// <summary> 火属性 </summary>
    Fire,
    /// <summary> 雷电属性 </summary>
    Lightning,
    /// <summary> 风属性 </summary>
    Wind,

    /// <summary> 结束标记 | 非属性 </summary>
    EndTag
}
