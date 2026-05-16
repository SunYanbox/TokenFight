using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 释放技能事件 </summary>
public struct ReleaseSkillContext : IContext
{
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 释放者 </summary>
    public IActor Source;
    /// <summary> 技能对象 </summary>
    public ISkill Skill;
}
