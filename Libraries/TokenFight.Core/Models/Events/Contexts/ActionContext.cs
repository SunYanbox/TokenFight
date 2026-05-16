using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 行动开始/结束事件上下文 </summary>
public struct ActionContext : IContext
{
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 成员 </summary>
    public IActor? Actor { get; set; }
    /// <summary> 执行的技能 </summary>
    public ISkill? Skill { get; set; }
    /// <summary> 是否是额外回合 </summary>
    public bool IsExtraTurn { get; set; }
    /// <summary> 是否是终结技 </summary>
    public bool IsUltimate { get; set; }
}