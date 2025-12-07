using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 效果应用与移除的事件上下文 </summary>
public struct EffectContext: IContext
{
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 施加伤害者 </summary>
    public IActor Source;
    /// <summary> 受到伤害者 </summary>
    public IActor Target;
    /// <summary> 应用的效果 </summary>
    public IEffect Effect;
    /// <summary> 是否是应用效果 </summary>
    public bool Apply;
}