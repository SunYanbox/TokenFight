using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 推条事件上下文 </summary>
public class PushContext: IContext
{
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 释放者 </summary>
    public required IActor Source;
    /// <summary> 被影响者 </summary>
    public required IActor Target;
    /// <summary> 推条的百分比(负数表示拉条) </summary>
    public double Adjust;
    /// <summary> 行动值变化量 </summary>
    public double ActionValueDelta;
}