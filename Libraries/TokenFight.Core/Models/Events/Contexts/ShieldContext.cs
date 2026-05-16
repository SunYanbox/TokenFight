using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 护盾事件上下文 </summary>
public struct ShieldContext : IContext
{
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 释放者 </summary>
    public IActor Source;
    /// <summary> 被影响者 </summary>
    public IActor Target;
    /// <summary> 护盾量 </summary>
    public double ShieldValue;
}
