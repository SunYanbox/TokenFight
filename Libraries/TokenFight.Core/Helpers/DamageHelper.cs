using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Actions;
using TokenFight.Core.Models.Entities.Masters;
using TokenFight.Core.Models.Utils;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Helpers;

/// <summary> 造成伤害的辅助工具 </summary>
[AutoSysRegistryInit]
public static class DamageHelper
{
    private static readonly Random Random = new();
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }
    public static readonly EnumTypeMaster<DamageType> DamageCommon = new([DamageType.NormalAttack]);
    /// <summary>
    /// 创建一个默认的伤害计算器 <br />
    /// - 基于攻击力倍率造成物理属性的普通攻击伤害 <br />
    /// - 结算增伤, 减防, 免伤, 易伤, 穿透与抗性, 暴击乘区
    /// </summary>
    public static readonly Func<DamageCalculator> DefaultCalculatorGet = () => DamageCalculator.New;

    /// <summary>
    /// 创建一个默认的真实伤害计算器 <br />
    /// - 基于固定值的真实伤害 <br />
    /// - 不会结算增伤, 减防, 免伤, 易伤, 穿透与抗性, 暴击乘区
    /// </summary>
    public static readonly Func<DamageCalculator> DefaultRealCalculatorGet = () =>
    {
        DamageCalculator calculator = DamageCalculator.New;
        calculator.WithFixBaseDamage(true)
            .WithCritical(false)
            .WithDamageIncrease(false)
            .WithDamageImmunity(false)
            .WithDefense(false)
            .WithVulnerability(false)
            .WithPenetrationAndResistance(false)
            .WithDamageTypes(DamageType.RealDamage);

        return calculator;
    };

    /// <summary> 判断成员是否可以正常计算属性 </summary>
    public static bool CanActorCalculateAttribute(IActor actor) => actor is { IsActive: true, Level: > 0 };

    /// <summary>
    /// 基于给定的数值造成伤害<br />
    /// 通过忽视护盾以造成真实伤害
    /// </summary>
    /// <returns>造成的总伤害</returns>
    public static double TakeDamageByConstant(IActor source, IActor target, double damage, ISkill? skill,
        bool isCrit = false, bool realDamage = false, EnumTypeMaster<DamageType>? damageTypes = null)
    {
        if (GameSystemRegistry == null) return 0;
        double beforeHealth = target.HealthMaster?.Health ?? 0;
        double oldHealth = target.HealthMaster?.Health ?? 0;

        BeforeExcuteContext? beforeExcuteContext = EventHelper.TriggerBeforeExcuteContext(EventType.DamageBefore, source, target, damage, skill);
        damage = beforeExcuteContext?.ValueCtx ?? damage;

        EnumTypeMaster<DamageType> dmgTypes = damageTypes ?? DamageCommon;
        if (realDamage)
        {
            target.HealthMaster!.TakeDamage(damage, out double overflowDamage);
            EventHelper.TriggerHealthChangeContext(target, (target.HealthMaster?.Health ?? 0) - oldHealth);
            EventHelper.TriggerDamageContext(source, target, dmgTypes, damage, 0, overflowDamage, isCrit,
                beforeHealth > 0 && target.HealthMaster?.Health <= double.Epsilon, true);
        }
        else
        {
            target.ShieldMaster!.TakeDamage(damage, out double residualDamage, out double shieldDefense);
            target.HealthMaster!.TakeDamage(residualDamage, out double overflowDamage);
            EventHelper.TriggerHealthChangeContext(target, (target.HealthMaster?.Health ?? 0) - oldHealth);
            EventHelper.TriggerDamageContext(source, target, dmgTypes, damage, shieldDefense, overflowDamage, isCrit,
                beforeHealth > 0 && target.HealthMaster?.Health <= double.Epsilon, false);
        }
        EventHelper.TriggerHealthChangeContext(target, (target.HealthMaster?.Health ?? 0) - beforeHealth);
        ActorHelper.HandleDeath(target);
        source.AttrSet.ClearTempModify();
        target.AttrSet.ClearTempModify();
        return damage;
    }

    /// <summary>
    /// 对指定单体造成基于自身某个基础乘区rate倍率的伤害
    /// </summary>
    /// <returns>造成的总伤害</returns>
    public static double TakeDirectDamageSingle(IActor source, IActor target, double rate, ISkill? skill,
        Func<DamageCalculator> calculatorGet)
    {
        if (GameSystemRegistry == null) return 0;
        if (!ActorHelper.IsValidActor(source) || !ActorHelper.IsValidActor(target)) return 0;

        DamageCalculator calculator = calculatorGet();
        EventHelper.TriggerBeforeCalculateContext(EventType.CalculateBefore, EventType.DamageBefore, source, target, skill);
        double damage = calculator.Calculate(source, target, rate, out bool isCrit);

        return TakeDamageByConstant(source, target, damage, skill, isCrit,
            calculator.DamageTypes.Contains(DamageType.RealDamage), calculator.DamageTypes);
    }

    /// <summary>
    /// 对指定单体造成一次伤害, 并根据这次造成的伤害对相邻单位造成一次固定值伤害, 最多迭代deep次
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="rate">基于基础乘区的倍率 / 固定值伤害</param>
    /// <param name="skill">用到的技能</param>
    /// <param name="calculatorGet">计算伤害的算法, 属性等</param>
    /// <param name="transform">转移给相邻目标的伤害量</param>
    /// <param name="deep">递归深度</param>
    /// <param name="isRateFix">是否按固定值结算伤害</param>
    /// <param name="inRecursion">是否在递归中</param>
    /// <returns>造成的总伤害</returns>
    public static double TakeDirectDamageTransform(IActor source, IActor target, double rate, ISkill? skill,
        Func<DamageCalculator> calculatorGet,
        double transform = 0.0, int deep = 1, bool isRateFix = false,
        bool inRecursion = false)
    {
        if (GameSystemRegistry == null) return 0;
        if (deep < 0) return 0;
        if (!ActorHelper.IsValidActor(source) || !ActorHelper.IsValidActor(target)) return 0;

        // 使用队列来模拟递归过程
        Queue<(IActor source, IActor target, double currentRate, int currentDeep, bool currentIsRateFix)> queue = new();
        queue.Enqueue((source, target, rate, deep, isRateFix));

        double totalDamage = 0;
        DamageCalculator calculator = calculatorGet();

        while (queue.Count > 0)
        {
            (IActor currentSource, IActor currentTarget, double currentRate, int currentDeep, bool currentIsRateFix) = queue.Dequeue();

            if (currentDeep < 0) continue;

            // 计算当前层的伤害
            EventHelper.TriggerBeforeCalculateContext(EventType.CalculateBefore, EventType.DamageBefore, currentSource, currentTarget, skill);

            bool isCrit = false;
            double damage = currentIsRateFix ? currentRate : calculator.Calculate(currentSource, currentTarget, currentRate, out isCrit);

            // 应用伤害并计算转移伤害
            double damageTransform = TakeDamageByConstant(currentSource, currentTarget, damage, skill, isCrit, calculator.DamageTypes.Contains(DamageType.RealDamage), calculator.DamageTypes) * transform;
            totalDamage += damage; // 累加当前伤害到总伤害

            // 如果还有剩余深度且需要转移伤害
            if (currentDeep > 0 && transform > double.Epsilon)
            {
                IActor? left = ActorHelper.GetActorFromWeakRef(currentTarget.RelationshipMaster.LeftActor);
                IActor? right = ActorHelper.GetActorFromWeakRef(currentTarget.RelationshipMaster.RightActor);

                // 将相邻单位加入队列进行下一轮处理
                if (ActorHelper.IsValidActor(left))
                {
                    queue.Enqueue((currentSource, left!, damageTransform, currentDeep - 1, true));
                }
                if (ActorHelper.IsValidActor(right))
                {
                    queue.Enqueue((currentSource, right!, damageTransform, currentDeep - 1, true));
                }
            }
        }

        // 清理临时属性修改
        if (!inRecursion)
            source.AttrSet.ClearTempModify();

        return totalDamage;
    }

    /// <summary>
    /// 对指定单体及其相邻单位造成指定伤害
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    /// <param name="rate">对主目标的倍率</param>
    /// <param name="rateNext">对相邻目标的倍率</param>
    /// <param name="skill">用到的技能</param>
    /// <param name="calculatorGet">计算伤害的算法, 属性等</param>
    /// <returns>造成的总伤害</returns>
    public static double TakeDirectDamageThree(IActor source, IActor target, double rate, double rateNext, ISkill? skill,
        Func<DamageCalculator> calculatorGet)
    {
        if (GameSystemRegistry == null) return 0;
        if (!ActorHelper.IsValidActor(source) || !ActorHelper.IsValidActor(target)) return 0;

        double damage = TakeDirectDamageSingle(source, target, rate, skill, calculatorGet);

        IActor? left = ActorHelper.GetActorFromWeakRef(target.RelationshipMaster.LeftActor);
        IActor? right = ActorHelper.GetActorFromWeakRef(target.RelationshipMaster.RightActor);

        if (ActorHelper.IsValidActor(left))
            damage += TakeDirectDamageSingle(source, left!, rateNext, skill, calculatorGet);
        if (ActorHelper.IsValidActor(right))
            damage += TakeDirectDamageSingle(source, right!, rateNext, skill, calculatorGet);
        return damage;
    }

    /// <summary>
    /// 对指定阵营全体造成一次伤害
    /// </summary>
    /// <returns>造成的总伤害</returns>
    public static double TakeDirectDamageAll(IActor source, TeamType team, double rate, ISkill? skill,
        Func<DamageCalculator> calculatorGet)
    {
        if (GameSystemRegistry == null) return 0;
        double sum = 0;
        foreach (IActor actor in ActorHelper.GetActorsByTeamAndValid(team))
        {
            if (ActorHelper.IsValidActor(actor))
            {
                sum += TakeDirectDamageSingle(source, actor, rate, skill, calculatorGet);
            }
        }

        return sum;
    }

    /// <summary>
    /// 对指定阵营全体造成times次弹射伤害
    /// </summary>
    /// <returns>造成的总伤害</returns>
    public static double TakeDirectDamageEjection(IActor source, TeamType team, double rate, ISkill? skill,
        Func<DamageCalculator> calculatorGet, int times = 1)
    {
        if (GameSystemRegistry == null) return 0;
        double sum = 0;
        for (int i = 0; i < times; i++)
        {
            IActor[] survivalActors = ActorHelper.GetActorsByTeamWithLifeAndValid(team);
            if (survivalActors.Length == 0) return sum;

            IActor actor = survivalActors[Random.Next(survivalActors.Length)];
            if (ActorHelper.IsValidActor(actor))
                sum += TakeDirectDamageSingle(source, actor, rate, skill, calculatorGet);
        }
        return sum;
    }
}
