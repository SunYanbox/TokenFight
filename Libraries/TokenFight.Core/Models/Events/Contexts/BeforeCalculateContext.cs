using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

public struct BeforeCalculateContext : IContext
{
    public EventType Type { get; init; }
    /// <summary> 下一件要做的事情 | 分辨是攻击前, 治疗前... </summary>
    public EventType NextType;
    public object Sender { get; init; }
    /// <summary> 释放者 </summary>
    public IActor Source;
    /// <summary> 被影响者 </summary>
    public IActor Target;
    /// <summary> 要修改的上下文数据 </summary>
    public double? Value;
    /// <summary> 正在使用的技能 </summary>
    public ISkill? Skill { get; init; }
}