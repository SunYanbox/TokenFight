using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 进入对局时的上下文 </summary>
public struct EnterGameContext : IContext
{
    /// <summary> 构造进入对局事件上下文 </summary>
    public EnterGameContext(IActor actor)
    {
        Type = EventType.EnterGame;
        Sender = actor;
        Actor = actor;
    }

    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 进入对局者 </summary>
    public IActor Actor { get; set; }
}
