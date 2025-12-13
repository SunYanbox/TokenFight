using TokenFight.Core.Constants;
using TokenFight.Core.Databases.Models.Growth;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Interfaces.Attrs;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Attrs;
using TokenFight.Core.Models.Effects.Skills;
using TokenFight.Core.Models.Entities.Actors;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Helpers;

/// <summary> 关于成员的一些工具类 </summary>
[AutoSysRegistryInit]
public static class ActorHelper
{
    private static readonly Random Random = new();
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }

    /// <summary> 根据传入的养成数据, 自动初始化生命值, 攻击力, 防御力, 速度 </summary>
    public static void InitAttrSet(IActor actor, DataActor dataActor)
    {
        Dictionary<int, double> modify = new();
        foreach ((AttrType type, GrowthBase value) in dataActor.AttrGrowth)
        {
            double v = value.Calculate(actor.Level);
            if (type == AttrType.Health)
                v = Math.Max(v, 1);
            modify.TryAdd(IAttrSet.ToInt(type), v);
        }
        actor.AttrSet?.SetAttr(new AttrModifyData
        {
            IsTemp = false,
            ModifyData = modify,
            Type = AttrModifyType.Base,
            Id = "base"
        });
    }

    /// <summary> 获取一个列表中的随意单位 </summary>
    public static IActor? GetRandomActor(IActor[] actors)
    {
        if (actors.Length == 0) return null;
        return actors[Random.Next(actors.Length)];
    }

    /// <summary> 连接父角色与召唤物的关系 </summary>
    public static void LinkParentAndChild(IActor parent, IActor child)
    {
        parent.RelationshipMaster.ChildActors ??= new Dictionary<string, WeakReference<IActor>>();
        parent.RelationshipMaster.ChildActors.Add(child.Id, new WeakReference<IActor>(child));
        child.RelationshipMaster.ParentActor = new WeakReference<IActor>(parent);
    }

    /// <summary> 从弱引用中提取值 </summary>
    public static IActor? GetActorFromWeakRef(WeakReference<IActor>? actor) => actor?.TryGetTarget(out IActor? target) ?? false ? target : null;

    /// <summary>
    /// 判断成员是否有效, 是否可以计算属性集
    /// </summary>
    public static bool IsValidActor(IActor? actor)
    {
        if (actor == null) return false;
        return DamageHelper.CanActorCalculateAttribute(actor);
    }

    /// <summary>
    /// 如果成员已经死亡, 触发一次OnDeath
    /// </summary>
    public static void HandleDeath(IActor actor)
    {
        if (!actor.IsLive())
        {
            actor?.OnDeath();
        }
    }

    /// <summary> 让成员被添加到战场中 </summary>
    public static void CreateActorToField(IActor actor)
    {
        EventHelper.TriggerActorLifeContext(actor, true);
    }

    /// <summary>
    /// 获取指定阵营的全体成员
    /// <remarks>适合群攻</remarks>
    /// </summary>
    public static IActor[] GetActorsByTeam(TeamType team)
    {
        if (GameSystemRegistry == null) return [];
        switch (team)
        {
            case TeamType.Player:
                return GameSystemRegistry.ActorManagerSystem.AllPlayers.Values.ToArray();
            case TeamType.Enemy:
                return GameSystemRegistry.ActorManagerSystem.AllEnemies.Values.ToArray();
        }
        return [];
    }

    /// <summary> 获取根据角色站位信息排序后的成员数组; 当所有成员都是同一阵营时有效 </summary>
    public static IActor[] SortActorsByPosition(IActor[] actors)
    {
        if (GameSystemRegistry == null || actors.Length <= 1) return actors;
        LinkedList<IActor> sortedActors = new();
        GameSystemRegistry.ActorPositionSystem.UpdateActorRelationship(GameSystemRegistry.ActorManagerSystem);
        IActor actor0 = actors[0];
        sortedActors.AddFirst(actor0);
        IActor? left = GetActorFromWeakRef(actor0.RelationshipMaster.LeftActor);
        IActor? right = GetActorFromWeakRef(actor0.RelationshipMaster.RightActor);
        while (left != null)
        {
            sortedActors.AddFirst(left);
            left = GetActorFromWeakRef(left.RelationshipMaster.LeftActor);
        }
        while (right != null)
        {
            sortedActors.AddLast(right);
            right = GetActorFromWeakRef(right.RelationshipMaster.RightActor);
        }
        return sortedActors.ToArray();
    }

    /// <summary>
    /// 获取指定阵营的通过验证的全体成员(如果获取不到, 会返回获取指定阵营的全体成员)
    /// <remarks>适合群攻</remarks>
    /// </summary>
    public static IActor[] GetActorsByTeamAndValid(TeamType team)
    {
        if (GameSystemRegistry == null) return [];
        IEnumerable<IActor> actors = [];
        switch (team)
        {
            case TeamType.Player:
                actors = GameSystemRegistry.ActorManagerSystem.AllPlayers.Values.Where(IsValidActor);
                break;
            case TeamType.Enemy:
                actors = GameSystemRegistry.ActorManagerSystem.AllEnemies.Values.Where(IsValidActor);
                break;
        }
        IActor[] actorsArray = actors.ToArray();
        return actorsArray.Length == 0
            ? GetActorsByTeam(team)
            : actorsArray;
    }

    /// <summary>
    /// 获取指定阵营的通过验证的全体存活成员(如果获取不到, 会返回获取指定阵营的全体成员)
    /// <remarks>适合弹射选择</remarks>
    /// </summary>
    public static IActor[] GetActorsByTeamWithLifeAndValid(TeamType team)
    {
        if (GameSystemRegistry == null) return [];
        IEnumerable<IActor> actors = [];
        switch (team)
        {
            case TeamType.Player:
                actors = GameSystemRegistry.ActorManagerSystem.AllPlayers.Values.Where(IsValidActor).Where(x => x.IsLive());
                break;
            case TeamType.Enemy:
                actors = GameSystemRegistry.ActorManagerSystem.AllEnemies.Values.Where(IsValidActor).Where(x => x.IsLive());
                break;
        }

        IActor[] actorsArray = actors.ToArray();

        return actorsArray.Length == 0
            ? GetActorsByTeam(team)
            : actorsArray;
    }

    /// <summary> 玩家阵营占场人员是否已满 </summary>
    public static bool IsPlayerFieldFull()
    {
        if (GameSystemRegistry == null) return false;
        return GameSystemRegistry.ActorManagerSystem.AllPlayers.Count >= GameConst.PlayerInFieldLimit;
    }

    /// <summary> 敌人阵营占场人员是否已满 </summary>
    public static bool IsEnemyFieldFull()
    {
        if (GameSystemRegistry == null) return false;
        return GameSystemRegistry.ActorManagerSystem.AllEnemies.Count >= GameConst.EnemyInFieldLimit;
    }

    /// <summary>
    /// 获取相对指定成员的所有队友, 可选排除自身
    /// </summary>
    public static IActor[] GetRelativeAllies(IActor actor, bool exclusion = false)
    {
        IActor[] result = GetActorsByTeamWithLifeAndValid(actor.Team);
        if (!exclusion) return result;
        return result.Where(x => x != actor).ToArray();
    }

    /// <summary>
    /// 获取相对指定成员的所有敌人
    /// </summary>
    /// <param name="actor">指定成员</param>
    public static IActor[] GetRelativeEnemies(IActor actor)
    {
        switch (actor.Team)
        {
            case TeamType.Player:
                return GetActorsByTeamWithLifeAndValid(TeamType.Enemy);
            case TeamType.Enemy:
                return GetActorsByTeamWithLifeAndValid(TeamType.Player);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /// <summary>
    /// 获取所有可用的, 未被选择的终结技
    /// </summary>
    public static List<IUltimateSkill> GetAllUltimateSkillCanUse()
    {
        if (GameSystemRegistry == null) return [];
        List<PlayerActor> readyActors = GameSystemRegistry.ActorManagerSystem.AllPlayers.Values
            .OfType<PlayerActor>()
            .Where(x => x.HasUltimateSkill() && x.GetUltimateSkill().CanUse()).ToList();

        List<IUltimateSkill> readyList = [];
        foreach (PlayerActor actor in readyActors)
        {
            if (actor is not { } playerActor) continue;
            ISkill skill = playerActor.GetUltimateSkill();
            if (skill is BaseUltimateSkill { IsUsing: false } ultimateSkill && ultimateSkill.CanUse())
            {
                readyList.Add(ultimateSkill);
            }
        }

        return readyList;
    }
}