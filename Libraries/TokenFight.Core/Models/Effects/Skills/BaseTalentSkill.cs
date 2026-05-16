using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Models.Effects.Skills;

public class BaseTalentSkill : BaseSkill
{
    /// <summary>
    /// 需要自行赋值Name和Desc | PassiveData已赋值
    /// </summary>
    [SetsRequiredMembers]
    protected BaseTalentSkill(string id, IActor source, GameSystemRegistry gameSystemRegistry)
    {
        Id = id;
        Name = "佚名";
        Source = source;
        Target = new WeakReference<IActor>(null!);
        Choice = SkillChoiceType.OnlySelf;
        Type = SkillType.NaturalTalent;
        AutoMakeSure = false;
        PassiveData = new PassiveData(gameSystemRegistry.EventSystem, gameSystemRegistry.LocalLog);
        Desc = "未知天赋";
    }

    public override bool CanUse() => false;

    public override void Execute() { }
}