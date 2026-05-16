using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> IActor创建与移除的上下文 </summary>
public struct ActorLifeContext : IContext
{
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 上下文成员 </summary>
    public IActor Actor;
    /// <summary> 是否是创建上下文 </summary>
    public bool IsCreate;
    /// <summary> 是否是移除上下文 </summary>
    public bool IsDestroy => !IsCreate;
}
