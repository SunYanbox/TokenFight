using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Models.Entities.Masters;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 造成伤害在事件中的上下文 </summary>
public struct DamageContext : IContext
{

    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 施加伤害者 </summary>
    public IActor Source;
    /// <summary> 受到伤害者 </summary>
    public IActor Target;
    /// <summary> 伤害种类 </summary>
    public EnumTypeMaster<DamageType> DamageType { get; init; }
    /// <summary> 造成的总伤害 </summary>
    public double Damage;
    /// <summary> 护盾抵消的伤害 </summary>
    public double ShieldDefense;
    /// <summary> 溢出伤害 </summary>
    public double OverflowDamage;
    /// <summary> 是否暴击 </summary>
    public bool IsKill;
    /// <summary> 是否真实伤害 </summary>
    public bool IsReal;
    /// <summary> 是否暴击 </summary>
    public bool IsCrit;
}
