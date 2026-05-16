using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 回合开始/结束事件上下文 </summary>
public struct RoundContext : IContext
{
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 成员 </summary>
    public IActor? Actor { get; set; }

}
