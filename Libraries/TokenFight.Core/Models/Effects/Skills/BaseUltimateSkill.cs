using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Models.Effects.Skills;

public abstract class BaseUltimateSkill: IUltimateSkill
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required IActor Source { get; set; }
    public required WeakReference<IActor> Target { get; set; }
    public required SkillChoiceType Choice { get; set; }
    public bool IsUsing { get; set; }

    public abstract bool CanUse();

    public bool KeepAction() => false;

    public abstract void Execute();

    public required PassiveData? PassiveData { get; set; }
    public bool AutoMakeSure { get; set; }
    public SkillType Type { get; set; } = SkillType.UltimateSkill;
}