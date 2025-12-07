using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Actions;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Helpers;

[AutoSysRegistryInit]
public static class HealHelper
{
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }
    private static readonly Random Random = new Random();
    
    /// <summary>
    /// 基于给定的数值提供治疗
    /// </summary>
    /// <returns>提供的总治疗</returns>
    public static double TakeHealByConstant(IActor source, IActor target, double heal, ISkill? skill)
    {
        if (GameSystemRegistry == null) return 0;
        if (!ActorHelper.IsValidActor(source) || !ActorHelper.IsValidActor(target)) return 0;
        double beforeHealth = target.HealthMaster?.Health ?? 0;
        BeforeExcuteContext? beforeExcuteContext = EventHelper.TriggerBeforeExcuteContext(EventType.HealBefore, source, target, heal, skill);
        heal = beforeExcuteContext?.ValueCtx ?? heal;
        target.HealthMaster!.TakeHeal(heal, out var overflowHeal);
        EventHelper.TriggerHealthChangeContext(target, (target.HealthMaster?.Health ?? 0) - beforeHealth);
        EventHelper.TriggerHealContext(source, target, heal, overflowHeal);
        return heal;
    }
    
    /// <summary>
    /// 对指定单体造成基于自身某个基础乘区rate倍率的治疗
    /// </summary>
    /// <returns>提供的总治疗</returns>
    public static double TakeHealSingle(IActor source, IActor target, double rate, ISkill? skill,
        AttrType baseFactor = AttrType.Health)
    {
        if (GameSystemRegistry == null) return 0;
        if (!ActorHelper.IsValidActor(source) || !ActorHelper.IsValidActor(target)) return 0;
        
        EventHelper.TriggerBeforeCalculateContext(EventType.CalculateBefore, EventType.HealBefore, source, target, skill);
        
        double heal = (source?.AttrSet?.GetAttr(baseFactor) ?? 0) * rate * (1 +
            (source?.AttrSet?.GetAttr(AttrType.HealIncrease) ?? 0));

        source!.AttrSet!.ClearTempModify();
        target.AttrSet!.ClearTempModify();
        
        return TakeHealByConstant(source, target, heal, skill);
    }
    
    /// <summary>
    /// 对指定阵营全体造成一次治疗
    /// </summary>
    /// <returns>提供的总治疗</returns>
    public static double TakeHealAll(IActor source, double rate, TeamType team, ISkill? skill, AttrType baseFactor = AttrType.Health)
    {
        if (GameSystemRegistry == null) return 0;
        double sum = 0;
        foreach (IActor actor in ActorHelper.GetActorsByTeamAndValid(team))
        {
            if (ActorHelper.IsValidActor(actor))
            {
                sum += TakeHealSingle(source, actor, rate, skill, baseFactor);
            }
        }

        return sum;
    }

    /// <summary>
    /// 对指定阵营全体造成times次弹射治疗
    /// </summary>
    /// <returns>提供的总治疗</returns>
    public static double TakeHealEjection(IActor source, double rate, TeamType team, ISkill? skill, int times = 1, AttrType baseFactor = AttrType.Health)
    {
        if (GameSystemRegistry == null) return 0;
        double sum = 0;
        for (int i=0; i<times; i++)
        {
            IActor[] survivalActors = ActorHelper.GetActorsByTeamWithLifeAndValid(team);
            if (survivalActors.Length == 0) return sum;

            IActor actor = survivalActors[Random.Next(survivalActors.Length)];
            if (ActorHelper.IsValidActor(actor))
                sum += TakeHealSingle(source, actor, rate, skill, baseFactor);
        }
        return sum;
    }
}