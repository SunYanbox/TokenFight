using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Actions;
using TokenFight.Core.Models.Entities.Masters;
using TokenFight.Core.Models.Events.Contexts;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Helpers;

[AutoSysRegistryInit]
public static class EventHelper
{
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }

    /// <summary>
    /// 触发SourceTargetContext上下文的事件
    /// <br />
    /// - 伤害结算前事件 <br />
    /// - 治疗结算前事件 <br />
    /// - 护盾结算前事件 <br />
    /// - 推条结算前事件
    /// </summary>
    public static BeforeExcuteContext? TriggerBeforeExcuteContext(EventType eventType, IActor source, IActor target, double value,
        ISkill? skill)
    {
        if (GameSystemRegistry == null) return null;
        try
        {
            BeforeExcuteContext beforeExcuteContext = new BeforeExcuteContext
            {
                Type = eventType,
                Sender = source,
                Source = source,
                Target = target,
                ValueCtx = value,
                Skill = skill
            };
            GameSystemRegistry.EventSystem.Trigger(eventType, beforeExcuteContext);
            return beforeExcuteContext;
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发事件{eventType.ToString()}时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }

        return null;
    }
    
    /// <summary>
    /// 计算双方角色属性前触发
    /// </summary>
    public static void TriggerBeforeCalculateContext(EventType eventType, EventType nextType, IActor source, IActor target,
        ISkill? skill)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                eventType,
                new BeforeCalculateContext
                {
                    Type = eventType,
                    NextType = nextType,
                    Sender = source,
                    Source = source,
                    Target = target,
                    Skill = skill
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发事件{eventType.ToString()}时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }
    
    /// <summary>
    /// 触发SourceTargetContext上下文的事件
    /// </summary>
    public static void TriggerSourceTargetContext(EventType eventType, IActor source, IActor target)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                eventType,
                new SourceTargetContext
                {
                    Type = eventType,
                    Sender = source,
                    Source = source,
                    Target = target
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发事件{eventType.ToString()}时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }

    /// <summary> 触发伤害事件 </summary>
    public static void TriggerDamageContext(IActor source, IActor target, EnumTypeMaster<DamageType> damageType, double damage, double shieldDefense,
        double overflowDamage, bool isCrit, bool isKill, bool isReal)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                EventType.Damage,
                new DamageContext
                {
                    Type = EventType.Damage,
                    Sender = source,
                    Source = source,
                    Target = target,
                    DamageType = damageType,
                    Damage = damage,
                    ShieldDefense = shieldDefense,
                    OverflowDamage = overflowDamage,
                    IsCrit = isCrit,
                    IsKill = isKill,
                    IsReal = isReal
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发伤害事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }
    
    /// <summary> 触发成员生命周期事件 </summary>
    public static void TriggerActorLifeContext(IActor actor, bool isCreate)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                EventType.ActorLife,
                new ActorLifeContext
                {
                    Type = EventType.ActorLife,
                    Sender = actor,
                    Actor = actor,
                    IsCreate = isCreate
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发生命周期事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }

    /// <summary> 触发效果事件 </summary>
    public static void TriggerEffectContext(IEffect effect, bool isApply)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            EventType type = isApply ? EventType.EffectApply : EventType.EffectRemove;
            if (!effect.Source.TryGetTarget(out IActor? source) ||
                !effect.Target.TryGetTarget(out IActor? target)) return;
            GameSystemRegistry.EventSystem.Trigger(
                type,
                new EffectContext
                {
                    Type = type,
                    Sender = source,
                    Source = source,
                    Target = target,
                    Effect = effect,
                    Apply = isApply
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发效果事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }
    
    /// <summary> 触发血量变化事件 </summary>
    public static void TriggerHealthChangeContext(IActor actor, double delta)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                EventType.HealthChange,
                new HealthChangeContext
                {
                    Type = EventType.HealthChange,
                    Sender = actor,
                    Actor = actor,
                    Delta = delta,
                    DeltaRatio = delta / Math.Max(actor.HealthMaster.HealthMax, 1)
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发效果事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }
    
    /// <summary> 触发治疗事件 </summary>
    public static void TriggerHealContext(IActor source, IActor target, double heal, double overflowHeal)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                EventType.Heal,
                new HealContext
                {
                    Type = EventType.Heal,
                    Sender = source,
                    Source = source,
                    Target = target,
                    HealValue = heal,
                    OverflowHealValue = overflowHeal
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发治疗事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }

    /// <summary>
    /// 触发推条事件
    /// </summary>
    /// <param name="source">源</param>
    /// <param name="target">目标</param>
    /// <param name="adjust">推条值(按百分比, 负数表示拉条)</param>
    /// <param name="delta">行动值变化量</param>
    public static void TriggerPushContext(IActor source, IActor target, double adjust, double delta)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                EventType.Push,
                new PushContext
                {
                    Type = EventType.Push,
                    Sender = source,
                    Source = source,
                    Target = target,
                    Adjust = adjust,
                    ActionValueDelta = delta
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发推条事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }
    
    /// <summary> 触发护盾事件 </summary>
    public static void TriggerShieldContext(IActor source, IActor target, double shield)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                EventType.Shield,
                new ShieldContext
                {
                    Type = EventType.Shield,
                    Sender = source,
                    Source = source,
                    Target = target,
                    ShieldValue = shield
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发护盾事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }
    
    /// <summary> 触发释放技能事件 </summary>
    public static void TriggerReleaseSkillContext(IActor source, ISkill skill)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                EventType.ReleaseSkill,
                new ReleaseSkillContext
                {
                    Type = EventType.Push,
                    Sender = source,
                    Source = source,
                    Skill = skill
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发释放技能事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }

    /// <summary> 触发行动开始或结束事件 </summary>
    public static void TriggerActionContext(EventType type, IActor? actor = null, ISkill? skill = null, bool isExtra = false,
        bool isUltimate = false)

    {
        if (actor == null && skill == null && type != EventType.ActionStart && type != EventType.ActionEnd) return;
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                type,
                new ActionContext
                {
                    Type = type,
                    Sender = actor,
                    Actor = actor,
                    Skill = skill,
                    IsExtraTurn = isExtra,
                    IsUltimate = isUltimate
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发行动开始或结束事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }
    
    /// <summary> 触发回合开始或结束事件 </summary>
    public static void TriggerRoundContext(EventType type, IActor actor)

    {
        if (type != EventType.RoundBegin && type != EventType.RoundEnd) return;
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(
                type,
                new RoundContext
                {
                    Type = type,
                    Sender = actor,
                    Actor = actor,
                });
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发回合开始或结束事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }
    
    /// <summary> 触发死亡事件 </summary>
    public static void TriggerDeathContext(IActor actor)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(EventType.ActorDeath, new DeathContext(actor));
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发死亡事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }

    /// <summary> 触发进入对局事件 </summary>
    public static void TriggerEnterGameContext(IActor actor)
    {
        if (GameSystemRegistry == null) return;
        try
        {
            GameSystemRegistry.EventSystem.Trigger(EventType.EnterGame, new EnterGameContext(actor));
        }
        catch (Exception e)
        {
            GameSystemRegistry.LocalLog.LogError($"触发进入对局事件时出错: {e.Message}\n\tstack: {e.StackTrace}");
        }
    }
}