using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Interfaces.FStream;
using TokenFight.Core.Interfaces.Systems.Combatant;
using TokenFight.Core.Models.Events.Contexts;

namespace TokenFight.Core.Models.Systems.Combatant;

public class ActorManagerSystem(
    IEventSystem eventSystem,
    ILocalLog localLog,
    IActorPositionSystem actorPositionSystem,
    IActionListSystem actionListSystem) : IActorManagerSystem
{
    private readonly Dictionary<string, IActor> _actors = new();
    private readonly HashSet<IActor> _deathActors = [];

    public void Init()
    {
        eventSystem.Subscribe(EventType.ActorLife, HandleActorLifeEvent);
        eventSystem.Subscribe(EventType.RoundBegin, HandleActorDeath);
        eventSystem.Subscribe(EventType.ActionStart, HandleActorDeath);
        eventSystem.Subscribe(EventType.ActionEnd, HandleActorDeath);
        eventSystem.Subscribe(EventType.ActorDeath, HandleActorDeath);
    }

    public void Reset()
    {
        foreach (IActor actor in _actors.Values)
        {
            actor.Destroy();
        }
        _actors.Clear();
    }

    public Dictionary<string, IActor> AllActors => _actors.AsReadOnly().ToDictionary();
    public Dictionary<string, IActor> AllPlayers =>
        _actors.AsReadOnly().Where(x => x.Value.Team == TeamType.Player).ToDictionary();
    public Dictionary<string, IActor> AllEnemies =>
        _actors.AsReadOnly().Where(x => x.Value.Team == TeamType.Enemy).ToDictionary();

    /// <summary> 处理成员创建与删除 </summary>
    private void HandleActorLifeEvent(IContext data)
    {
        if (data is ActorLifeContext actorLifeContext)
        {
            if (actorLifeContext.IsCreate)
            {
                localLog.Debug($"[成员管理系统] Actor {actorLifeContext.Actor.Name}({actorLifeContext.Actor.Id}) 已创建");
                _actors.Add(actorLifeContext.Actor.Id, actorLifeContext.Actor);
                actionListSystem.Append(actorLifeContext.Actor);
                actorPositionSystem.Append(actorLifeContext.Actor.Id, actorLifeContext.Actor.Team);
                actorLifeContext.Actor.OnEnterGame();
                EventHelper.TriggerEnterGameContext(actorLifeContext.Actor);
            }
            else
            {
                localLog.Debug($"[成员管理系统] Actor {actorLifeContext.Actor.Name}({actorLifeContext.Actor.Id}) 已移除");
                _actors.Remove(actorLifeContext.Actor.Id);
            }
        }
    }

    private void HandleActorDeath(IContext data)
    {
        // 击败时不会直接销毁, 等到行动完全结束或回合开始时统一销毁
        if (data is DeathContext deathContext)
        {
            if (deathContext.Type == EventType.ActorDeath)
            {
                _deathActors.Add(deathContext.Actor);
            }
            return;
        }

        HashSet<IActor> delayDeathActors = [];
        // 移除该被销毁的成员
        if (data is ActionContext or RoundContext)
        {
            if (_deathActors.Count == 0) return;
            foreach (var actor in _deathActors)
            {
                if (actor.DelayDeath)
                {
                    delayDeathActors.Add(actor);
                    continue;
                }
                // 移除行动
                // TODO: 清理引用
                // actionManagerSystem.RemoveWithActor(actor);
                actionListSystem.Remove(actor);
                // 去掉关系
                if (actor?.RelationshipMaster?.ParentActor != null)
                {
                    // TODO: 清理引用
                    // actor.RelationshipMaster.ParentActor.RelationshipMaster.ChildActors?.Remove(actor.RelationshipMaster.ParentActor.Id);
                }
                if (!string.IsNullOrEmpty(actor!.Id)) actorPositionSystem.Remove(actor.Id, actor.Team);
                actorPositionSystem.UpdateActorRelationship(this);
                foreach (IActor mem in AllActors.Values)
                {
                    // mem.RemoveWithActor(actor);
                }
                if (actor.Team == TeamType.Player && actor.RelationshipMaster?.ParentActor == null) // 召唤物可以被清除
                {
                    delayDeathActors.Add(actor);
                    continue;
                }
                // 销毁成员
                actor.Destroy();
                _actors.Remove(actor.Id);
                EventHelper.TriggerActorLifeContext(actor, false);
            }
            _deathActors.Clear();
            _deathActors.UnionWith(delayDeathActors);
        }
    }
}
