using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Models.Effects.Skills;

/// <summary> 技能基类 </summary>
public abstract class BaseSkill : ISkill
{
    public required string Id { get; set; }

    public required string Name { get; set; }

    public required string Desc { get; set; }

    public required IActor Source { get; set; }

    public required WeakReference<IActor> Target { get; set; }

    public required SkillChoiceType Choice { get; set; }

    public abstract bool CanUse();

    public virtual bool KeepAction() => false;

    public abstract void Execute();

    public PassiveData? PassiveData { get; set; }

    public bool AutoMakeSure { get; set; }

    public required SkillType Type { get; set; }
}
