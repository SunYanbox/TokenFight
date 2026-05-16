using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 源 + 目标 事件上下文 </summary>
public struct SourceTargetContext : IContext
{
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 释放者 </summary>
    public IActor Source;
    /// <summary> 被影响者 </summary>
    public IActor Target;
}
