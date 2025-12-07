using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Models.Effects.Skills;

/// <summary> 基于攻击力的指定比率对指定敌方单体造成伤害 | 可选消耗或增加战技点 </summary>
public class BasicBasicSkill: BaseSkill
{
    protected double Rate;
    protected int SkillPointDelta;
    protected double EnergyCharge;
    protected readonly GameSystemRegistry GameSystemRegistry;
    
    [SetsRequiredMembers]
    public BasicBasicSkill(string id, IActor source, double rate, GameSystemRegistry gameSystemRegistry, 
        string name = "佚名", string desc = "未知", int skillPointDelta = 0, double charge = 0)
    {
        Id = id;
        Source = source;
        Target = new WeakReference<IActor>(null!);
        Name = name;
        Desc = desc;
        Choice = SkillChoiceType.OnlyEnemy;
        PassiveData = new PassiveData(gameSystemRegistry.EventSystem, gameSystemRegistry.LocalLog);
        AutoMakeSure = false;
        Type = SkillType.BasicAttack;
        
        Rate = rate;
        SkillPointDelta = skillPointDelta;
        GameSystemRegistry = gameSystemRegistry;
        EnergyCharge = charge;
    }

    public override bool CanUse() => true;

    public override void Execute()
    {
        if (ActorHelper.IsValidActor(Source))
        {
            IActor? target = ActorHelper.GetActorFromWeakRef(Target);
            if (ActorHelper.IsValidActor(target))
                DamageHelper.TakeDirectDamageSingle(Source, target!, Rate, this, DamageHelper.DefaultCalculatorGet);
            
            if (Math.Abs(EnergyCharge) > Double.Epsilon)
            {
                Source.EnergyMaster.Adjust(EnergyCharge);
            }
        }

        if (SkillPointDelta != 0)
        {
            GameSystemRegistry.GlobalResourcesSystem.AdjustSkillPoint(SkillPointDelta);
        }
    }
}