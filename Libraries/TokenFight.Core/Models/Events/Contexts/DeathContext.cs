using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 死亡事件上下文 </summary>
public struct DeathContext : IContext
{
    /// <summary> 构造死亡事件上下文 </summary>
    public DeathContext(IActor actor)
    {
        Type = EventType.ActorDeath;
        Sender = actor;
        Actor = actor;
    }
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 施加伤害者 </summary>
    public IActor Actor { get; set; }
}
