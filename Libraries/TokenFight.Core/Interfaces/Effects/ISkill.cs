using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models.Effects;

namespace TokenFight.Core.Interfaces.Effects;

/// <summary> 技能接口 </summary>
public interface ISkill
{
    /// <summary> 技能Id | 动态 </summary>
    public string Id { get; set; }
    /// <summary> 技能名称 </summary>
    public string Name { get; set; }
    /// <summary> 技能描述 </summary>
    public string Desc { get; set; }
    /// <summary> 技能释放者 </summary>
    public IActor Source { get; set; }
    /// <summary> 技能目标 </summary>
    public WeakReference<IActor> Target { get; set; }
    /// <summary> 选择技能目标方式 </summary>
    public SkillChoiceType Choice { get; set; }
    /// <summary> 获取技能是否可用 </summary>
    public bool CanUse();
    /// <summary> 保持连续行动 </summary>
    public bool KeepAction();
    /// <summary> 执行技能 </summary>
    public void Execute();
    /// <summary> 被动技能 </summary>
    public PassiveData? PassiveData { get; set; }
    /// <summary> 自动确认 </summary>
    public bool AutoMakeSure { get; set; }
    /// <summary> 优先级 </summary>
    public SkillType Type { get; set; }
}
