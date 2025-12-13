using TokenFight.Core.Enums.Events;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Interfaces.Systems;
using TokenFight.Core.Models.Events.Contexts;

namespace TokenFight.Core.Models.Systems;

public class EventOutputSystem(IEventSystem eventSystem): IEventOutputSystem
{
    public void Init()
    {
        foreach (EventType e in Enum.GetValues<EventType>())
        {
            eventSystem.Subscribe(e, OutputHelper.OutputGameSummary);
        }
    }

    public void Reset()
    {

    }
}

internal static class OutputHelper
{
    // 颜色配置
    private static readonly Dictionary<EventType, ConsoleColor> EventColors = new()
    {
        [EventType.Damage] = ConsoleColor.Red,
        [EventType.Heal] = ConsoleColor.Green,
        [EventType.Shield] = ConsoleColor.Cyan,
        [EventType.EffectApply] = ConsoleColor.Yellow,
        [EventType.EffectRemove] = ConsoleColor.DarkYellow,
        [EventType.Push] = ConsoleColor.Magenta,
        [EventType.EnterGame] = ConsoleColor.DarkGreen,
        [EventType.ReleaseSkill] = ConsoleColor.Blue,
        [EventType.ActionStart] = ConsoleColor.White,
        [EventType.ActionEnd] = ConsoleColor.Gray,
        [EventType.RoundBegin] = ConsoleColor.DarkCyan,
        [EventType.RoundEnd] = ConsoleColor.DarkGray,
        [EventType.ActorDeath] = ConsoleColor.DarkRed,
        [EventType.ActorLife] = ConsoleColor.DarkGreen
    };

    // 图标配置
    private static readonly Dictionary<EventType, string> EventIcons = new()
    {
        [EventType.Damage] = "[ATK]", // Attack
        [EventType.Heal] = "[HEAL]", // Heal
        [EventType.Shield] = "[DEF]", // Defense
        [EventType.EffectApply] = "[+]", // Add effect
        [EventType.EffectRemove] = "[-]", // Remove effect
        [EventType.Push] = "[>>]", // Push action
        [EventType.EnterGame] = "[ENTER]", // Push action
        [EventType.ReleaseSkill] = "[SKILL]", // Skill
        [EventType.ActionStart] = "[START]", // Action start
        [EventType.ActionEnd] = "[END]", // Action end
        [EventType.RoundBegin] = "[ROUND+]", // Round begin
        [EventType.RoundEnd] = "[ROUND-]", // Round end
        [EventType.ActorDeath] = "[DEAD]", // Death
        [EventType.ActorLife] = "[LIFE]" // Life cycle
    };

    /// <summary>
    /// 输出游戏事件的简略彩色信息到控制台
    /// </summary>
    /// <param name="data">事件数据上下文</param>
    public static void OutputGameSummary(IContext data)
    {
        EventType gameEvent = data.Type;

        ConsoleColor color = GetEventColor(gameEvent);
        string icon = GetEventIcon(gameEvent);

        Console.ForegroundColor = color;

        try
        {
            switch (gameEvent)
            {
                case EventType.Damage when data is DamageContext damageContext:
                    OutputDamage(damageContext.Source, damageContext, icon);
                    break;
                case EventType.Heal when data is HealContext healContext:
                    OutputHeal(healContext.Source, healContext, icon);
                    break;
                case EventType.Shield when data is ShieldContext shieldContext:
                    OutputShield(shieldContext.Source, shieldContext, icon);
                    break;
                case EventType.EffectApply when data is EffectContext effectContext:
                    OutputEffectApply(effectContext.Source, effectContext, icon);
                    break;
                case EventType.EffectRemove when data is EffectContext effectContext:
                    OutputEffectRemove(effectContext.Source, effectContext, icon);
                    break;
                case EventType.Push when data is PushContext pushContext:
                    OutputPush(pushContext.Source, pushContext, icon);
                    break;
                case EventType.EnterGame when data is EnterGameContext enterGameContext:
                    OutputEnterGame(enterGameContext.Actor, enterGameContext, icon);
                    break;
                case EventType.ReleaseSkill when data is ReleaseSkillContext releaseSkillContext:
                    OutputSkillRelease(releaseSkillContext.Source, releaseSkillContext, icon);
                    break;
                case EventType.ActionStart when data is ActionContext actionContext:
                    OutputActionStart(actionContext.Actor!, actionContext, icon);
                    break;
                // case TriggerType.ActionEnd when data is ActionContext actionContext:
                //     OutputActionEnd(actionContext.Actor!, actionContext, icon);
                //     break;
                // case TriggerType.RoundBegin when data is RoundContext:
                //     OutputRoundBegin(RoundContext.Source, icon);
                //     break;
                // case TriggerType.RoundEnd when data is RoundContext:
                //     OutputRoundEnd(RoundContext.Source, icon);
                //     break;
                case EventType.ActorDeath when data is DeathContext deathContext:
                    OutputDeath(deathContext.Actor, icon);
                    break;
                // case TriggerType.ActorLife when data is ActorLifeContext actorLifeContext:
                //     OutputActorLife(actorLifeContext.Actor, actorLifeContext, icon);
                //     break;
            }
        }
        finally
        {
            Console.ResetColor();
        }
    }

    private static void OutputEnterGame(IActor actor, EnterGameContext context, string icon)
    {
        Console.WriteLine($"{icon} {actor.Name}({actor.Id})进入对局");
    }

    private static void OutputDamage(IActor actor, DamageContext context, string icon)
    {
        string damageType = string.Join("|", context.DamageType.ToArray());
        string critMark = context.IsCrit ? "暴击" : "";
        string killMark = context.IsKill ? "击杀" : "";
        string realMark = context.IsReal ? "真实" : "";
        Console.WriteLine($"{icon} {actor.Name}({actor.Id}) → {context.Target.Name}({context.Target.Id}): {context.Damage:F1}伤害 {{ {damageType} }} {critMark} {killMark} {realMark}");
        if (context.ShieldDefense > 0)
            Console.WriteLine($"   护盾吸收: {context.ShieldDefense:F1}");
    }

    private static void OutputHeal(IActor actor, HealContext context, string icon)
    {
        Console.WriteLine($"{icon} {actor.Name}({actor.Id}) → {context.Target.Name}({context.Target.Id}): +{context.HealValue:F1}治疗");
        if (context.OverflowHealValue > 0)
            Console.WriteLine($"   过量治疗: {context.OverflowHealValue:F1}");
    }

    private static void OutputShield(IActor actor, ShieldContext context, string icon)
    {
        Console.WriteLine($"{icon} {actor.Name}({actor.Id}) → {context.Target.Name}({context.Target.Id}): +{context.ShieldValue:F1}护盾");
    }

    private static void OutputEffectApply(IActor actor, EffectContext context, string icon)
    {
        IEffect effect = context.Effect;
        string stackInfo = effect.LifeCycle?.HasStack ?? false ? $"({effect.LifeCycle.CurrentStack}层)" : "";
        string markInfo = effect.LifeCycle?.HasMark ?? false ? $"({effect.LifeCycle.CurrentMark}层)" : "";
        IActor? target = ActorHelper.GetActorFromWeakRef(context.Effect.Target);
        Console.WriteLine($"{icon} {actor.Name}({actor.Id}) -> {target?.Name}({target?.Id}): {effect.Id} {stackInfo} {markInfo}");
    }

    private static void OutputEffectRemove(IActor actor, EffectContext context, string icon)
    {
        IEffect effect = context.Effect;
        IActor? target = ActorHelper.GetActorFromWeakRef(context.Effect.Target);
        Console.WriteLine($"{icon} -> {target?.Name}({target?.Id}): 移除{effect.Id}");
    }

    private static void OutputPush(IActor actor, PushContext context, string icon)
    {
        string actionType = context.Adjust > 0 ? "推迟" : "提前";
        string percentage = Math.Abs(context.Adjust).ToString("P0");
        Console.WriteLine($"{icon} {actor.Name}({actor.Id}) {context.Target.Name}({context.Target.Id}): {actionType}{percentage} 行动值变化量: {context.ActionValueDelta:F2}");
    }

    private static void OutputSkillRelease(IActor actor, ReleaseSkillContext context, string icon)
    {
        Console.WriteLine($"{icon} {actor.Name}({actor.Id}) 释放: {context.Skill.Name}");
        Console.WriteLine($"   目标: {context.Skill.Choice}");
    }

    private static void OutputActionStart(IActor actor, ActionContext context, string icon)
    {
        List<string> typeMarks = [];
        if (context.IsExtraTurn) typeMarks.Add("额外回合");
        if (context.IsUltimate) typeMarks.Add("终结技");

        string typeInfo = typeMarks.Count > 0 ? $" [{string.Join("+", typeMarks)}]" : "";
        Console.WriteLine($"\n{icon} {actor.Name}({actor.Id}) 开始行动{typeInfo}");

        if (context.Skill != null)
            Console.WriteLine($"   技能: {context.Skill.Name}  目标: {ActorHelper.GetActorFromWeakRef(context.Skill.Target)?.Name}");
    }

    private static void OutputActionEnd(IActor actor, ActionContext context, string icon)
    {
        Console.WriteLine($"{icon} {actor.Name}({actor.Id}) 结束行动");
    }

    private static void OutputRoundBegin(IActor actor, string icon)
    {
        Console.WriteLine($"{icon} === {actor.Name}({actor.Id}) 回合开始 ===");
    }

    private static void OutputRoundEnd(IActor actor, string icon)
    {
        Console.WriteLine($"{icon} === {actor.Name}({actor.Id}) 回合结束 ===");
    }

    private static void OutputDeath(IActor actor, string icon)
    {
        Console.WriteLine($"{icon} {actor.Name}({actor.Id}) 阵亡");
    }

    private static void OutputActorLife(IActor actor, ActorLifeContext context, string icon)
    {
        string action = context.IsDestroy ? "销毁" : "创建";
        Console.WriteLine($"{icon} {actor.Name}({actor.Id}) {action}");
    }

    private static ConsoleColor GetEventColor(EventType eventType) => EventColors.TryGetValue(eventType, out ConsoleColor color) ? color : ConsoleColor.White;

    private static string GetEventIcon(EventType eventType) => EventIcons.TryGetValue(eventType, out string? icon) ? icon : "[标记]";
}