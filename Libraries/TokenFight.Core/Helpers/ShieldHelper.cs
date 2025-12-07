using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Actions;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Helpers;

/// <summary> 护盾工具类 </summary>
[AutoSysRegistryInit]
public static class ShieldHelper
{
    private static readonly Random Random = new();
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }
    
    /// <summary>
    /// 基于给定的数值提供护盾
    /// </summary>
    /// <returns>提供的总护盾</returns>
    public static double TakeShieldByConstant(IActor source, IActor target, double shield, string shieldUuid, ISkill? skill, bool stackable = false)
    {
        if (GameSystemRegistry == null) return 0;
        if (!ActorHelper.IsValidActor(source) || !ActorHelper.IsValidActor(target)) return 0;
        BeforeExcuteContext? beforeExcuteContext 
            = EventHelper.TriggerBeforeExcuteContext(EventType.ShieldBefore, source, target, shield, skill);
        shield = beforeExcuteContext?.ValueCtx ?? shield;
        if (stackable)
        {
            shield += target.ShieldMaster!.GetShield(shieldUuid);
        }
        target.ShieldMaster!.Add(shieldUuid, shield);
        EventHelper.TriggerShieldContext(source, target, shield);
        return shield;
    }
    
    /// <summary>
    /// 对指定单体造成基于自身某个基础乘区rate倍率的护盾
    /// </summary>
    /// <returns>提供的总护盾</returns>
    public static double TakeShieldSingle(IActor source, IActor target, double rate,
        string shieldUuid, ISkill? skill, bool stackable = false, 
        AttrType baseFactor = AttrType.Defense)
    {
        if (GameSystemRegistry == null) return 0;
        if (!ActorHelper.IsValidActor(source) || !ActorHelper.IsValidActor(target)) return 0;
        
        EventHelper.TriggerBeforeCalculateContext(EventType.CalculateBefore, EventType.ShieldBefore, source, target, skill);
        
        double shield = (source?.AttrSet?.GetAttr(baseFactor) ?? 0) * rate * (1 +
            (source?.AttrSet?.GetAttr(AttrType.ShieldIncrease) ?? 0));

        source!.AttrSet!.ClearTempModify();
        target.AttrSet!.ClearTempModify();
        
        return TakeShieldByConstant(source, target, shield, shieldUuid, skill, stackable);
    }
    
    /// <summary>
    /// 对指定阵营全体提供一次护盾
    /// </summary>
    /// <returns>提供的总治疗</returns>
    public static double TakeShieldAll(IActor source, double rate, TeamType team, string shieldUuid, ISkill? skill, bool stackable = false, AttrType baseFactor = AttrType.Health)
    {
        if (GameSystemRegistry == null) return 0;
        double sum = 0;
        foreach (IActor actor in ActorHelper.GetActorsByTeamAndValid(team))
        {
            if (ActorHelper.IsValidActor(actor))
            {
                sum += TakeShieldSingle(source, actor, rate, shieldUuid, skill, stackable, baseFactor);
            }
        }

        return sum;
    }

    /// <summary>
    /// 对指定阵营全体提供times次弹射护盾
    /// </summary>
    /// <returns>提供的总治疗</returns>
    public static double TakeShieldEjection(IActor source, double rate, TeamType team, string shieldUuid, ISkill? skill, bool stackable = false, int times = 1, AttrType baseFactor = AttrType.Health)
    {
        if (GameSystemRegistry == null) return 0;
        double sum = 0;
        for (int i=0; i<times; i++)
        {
            IActor[] survivalActors = ActorHelper.GetActorsByTeamWithLifeAndValid(team);
            if (survivalActors.Length == 0) return sum;

            IActor actor = survivalActors[Random.Next(survivalActors.Length)];
            if (ActorHelper.IsValidActor(actor))
                sum += TakeShieldSingle(source, actor, rate, shieldUuid, skill, stackable, baseFactor);
        }
        return sum;
    }
}