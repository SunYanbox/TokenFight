using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Actions;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Helpers;

/// <summary>
/// 推条工具类
/// </summary>
[AutoSysRegistryInit]
public static class PushHelper
{
    private static readonly Random Random = new();
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }

    /// <summary>
    /// 使得指定目标行动提前或延后
    /// rate>0时推条, rate小于0时拉条
    /// </summary>
    public static void TakePushSingle(IActor source, IActor target, double rate, ISkill? skill)
    {
        if (GameSystemRegistry == null) return;
        if (!ActorHelper.IsValidActor(source) || !ActorHelper.IsValidActor(target)) return;

        BeforeExcuteContext? beforeExcuteContext = EventHelper.TriggerBeforeExcuteContext(EventType.PushBefore, source, target, rate, skill);
        rate = beforeExcuteContext?.ValueCtx ?? rate;
        double action = target.ActionValueMaster.ActionValue;
        target.ActionValueMaster.Push(rate);

        source.AttrSet.ClearTempModify();
        target.AttrSet.ClearTempModify();

        EventHelper.TriggerPushContext(source, target, rate, target.ActionValueMaster.ActionValue - action);
    }

    /// <summary>
    /// 使得指定目标行动立即行动
    /// </summary>
    public static void TakeSingleActionNow(IActor source, IActor target, ISkill? skill)
    {
        if (GameSystemRegistry == null) return;
        if (!ActorHelper.IsValidActor(source) || !ActorHelper.IsValidActor(target)) return;

        TakePushSingle(source, target, -1.0, skill);
        CreateActionHelper.CreateNormal(target);
    }

    /// <summary>
    /// 使得指定阵营全体目标行动提前或延后
    /// rate>0时推条, rate小于0时拉条
    /// </summary>
    /// <returns>提供的总护盾</returns>
    public static void TakePushAll(IActor source, TeamType team, double rate, ISkill? skill)
    {
        if (GameSystemRegistry == null) return;
        foreach (IActor actor in ActorHelper.GetActorsByTeamAndValid(team))
        {
            if (ActorHelper.IsValidActor(actor))
            {
                TakePushSingle(source, actor, rate, skill);
            }
        }
    }

    /// <summary>
    /// 对我方全体造成times次弹射行动提前或延后
    /// rate>0时推条, rate小于0时拉条
    /// </summary>
    /// <returns>提供的总护盾</returns>
    public static void TakePushEjection(IActor source, double rate, TeamType team, ISkill? skill, int times = 1)
    {
        if (GameSystemRegistry == null) return;
        for (int i = 0; i < times; i++)
        {
            IActor[] survivalActors = ActorHelper.GetActorsByTeamWithLifeAndValid(team);
            if (survivalActors.Length == 0) return;

            IActor actor = survivalActors[Random.Next(survivalActors.Length)];
            if (ActorHelper.IsValidActor(actor))
                TakePushSingle(source, actor, rate, skill);
        }
    }
}