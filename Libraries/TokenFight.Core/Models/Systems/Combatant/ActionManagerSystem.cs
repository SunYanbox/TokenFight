using TokenFight.Core.Enums.Actions;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.FStream;
using TokenFight.Core.Interfaces.Systems.Combatant;
using TokenFight.Core.Models.Actions;

namespace TokenFight.Core.Models.Systems.Combatant;

public class ActionManagerSystem(ILocalLog localLog): IActionManagerSystem
{
    public void Init() { }

    public void Reset()
    {
        _newestAction = null;
        foreach (List<ActionUnit> action in _actionsByPriority.Values)
        {
            action.Clear();
        }
        _alreadyTakenActions.Clear();
        _handledRoundStart.Clear();
        _actionsByActorId.Clear();
        _actionNextIndex = 0;
    }

    public bool CreateAction(ActionPriority priority, IActor? actor = null, ISkill? skill = null)
    {
        string id = actor?.Id ?? skill!.Source.Id;
        if (!_actionsByActorId.ContainsKey(id)) _actionsByActorId.Add(id, []);
        switch (priority)
        {
            case ActionPriority.FollowUpAttack:
                _actionsByActorId[id].Add(ActionPriority.FollowUpAttack);
                _actionsByPriority[ActionPriority.FollowUpAttack].Add(new ActionUnit
                {
                    ActionEnd = false,
                    CreateId = _actionNextIndex++,
                    Skill = skill,
                    Priority = ActionPriority.FollowUpAttack
                });
                return true;
            case ActionPriority.ExtraTurn:
                _actionsByActorId[id].Add(ActionPriority.ExtraTurn);
                _actionsByPriority[ActionPriority.ExtraTurn].Add(new ActionUnit
                {
                    ActionEnd = false,
                    CreateId = _actionNextIndex++,
                    Actor = actor,
                    Priority = ActionPriority.ExtraTurn
                });
                return true;
            case ActionPriority.Ultimate:
                _actionsByActorId[id].Add(ActionPriority.Ultimate);
                _actionsByPriority[ActionPriority.Ultimate].Add(new ActionUnit
                {
                    ActionEnd = false,
                    CreateId = _actionNextIndex++,
                    Skill = skill,
                    Priority = ActionPriority.Ultimate
                });
                return true;
            case ActionPriority.NormalOperations:
                if (_actionsByActorId[id].Add(ActionPriority.NormalOperations))
                {
                    _actionsByPriority[ActionPriority.NormalOperations].Add(new ActionUnit
                    {
                        ActionEnd = false,
                        CreateId = _actionNextIndex++,
                        Actor = actor,
                        Priority = ActionPriority.NormalOperations
                    });
                    return true;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
        }

        return false;
    }

    #region 内部数据
    private readonly Dictionary<ActionPriority, List<ActionUnit>> _actionsByPriority
        = new()
        {
            { ActionPriority.FollowUpAttack, [] },
            { ActionPriority.ExtraTurn, [] },
            { ActionPriority.Ultimate, [] },
            { ActionPriority.NormalOperations, [] }
        };
    private readonly Dictionary<string, HashSet<ActionPriority>> _actionsByActorId = new();
    private readonly HashSet<ActionUnit> _handledRoundStart = [];
    private readonly HashSet<ActionUnit> _alreadyTakenActions = []; // TODO: 已行动单位清理
    private ActionUnit? _newestAction = null;
    private int _actionNextIndex = 0;
    #endregion


    public bool Empty => _actionsByPriority.Values.Select(x => x.Count).Sum() == 0;
    public ActionUnit? NewestAction
    {
        get
        {
            if (_newestAction == null)
            {
                UpdateNewestAction();
            }
            else
            {
                // 存在优先行动可以插队普通行动
                if (_newestAction.IsNormal && _actionsByPriority.Where(
                            x => x.Key != ActionPriority.NormalOperations)
                        .Select(x => x.Value.Count).Sum() > 0)
                {
                    UpdateNewestAction();
                }
            }
            return _newestAction;
        }
    }
    public List<ActionUnit> GetActionList()
    {
        List<ActionUnit> result = [];
        result.AddRange(_actionsByPriority[ActionPriority.FollowUpAttack]);
        result.AddRange(MergeByCreateId(_actionsByPriority[ActionPriority.ExtraTurn], _actionsByPriority[ActionPriority.Ultimate]));
        result.AddRange(_actionsByPriority[ActionPriority.NormalOperations]);
        return result;
    }

    public void RoundBegin()
    {
        if (!CanSettleRound(NewestAction)) return;
        if (NewestAction == null) return;
        if (_handledRoundStart.Add(NewestAction))
        {
            // 如果元素被添加到HashSet<T>对象中，则返回true；如果元素已存在，则返回false。
            NewestAction?.Actor!.RoundBegin();
            EventHelper.TriggerRoundContext(EventType.RoundBegin, NewestAction?.Actor!);
        }
    }

    public void RoundEnd()
    {
        foreach (ActionUnit actionUnit in _handledRoundStart.ToArray())
        {
            if (_alreadyTakenActions.Contains(actionUnit))
            {
                _handledRoundStart.Remove(actionUnit);
            }
            actionUnit.Actor!.RoundEnd();
            EventHelper.TriggerRoundContext(EventType.RoundEnd, actionUnit.Actor);
        }
    }

    public (IActor? actor, ISkill? skill) ActionBegin()
    {
        if (Empty) return new ValueTuple<IActor?, ISkill?>(null, null);
        ActionUnit actionUnit = NewestAction!;
        (IActor? actor, ISkill? skill) = GetActorAndSkillFromNewestAction();

        if (actor == null)
        {
            localLog.Warn($"[ActionManagerSystem.ExecuteAction] 无法找到执行本次行动的成员" +
                          $"\n\tactor: {actionUnit?.Actor}" +
                          $"\n\tskill: {actionUnit?.Skill}");
        }

        if (actionUnit!.IsNormal)
            actor?.ActionValueMaster?.Reset();

        RemoveFromPriId();

        EventHelper.TriggerActionContext(EventType.ActionStart, actor, skill, actionUnit.IsExtraTurn,
            actionUnit.IsUltimate);
        return new ValueTuple<IActor?, ISkill?>(actor, skill);
    }

    public void ActionEnd()
    {
        if (_newestAction != null)
        {
            _alreadyTakenActions.Add(_newestAction);
        }

        (IActor? actor, ISkill? skill) = GetActorAndSkillFromNewestAction();

        if (actor == null) return;

        EventHelper.TriggerActionContext(EventType.ActionEnd, actor, skill, _newestAction!.IsExtraTurn, _newestAction.IsUltimate);
        _newestAction = null;
    }

    #region 内部工具函数
    // 当前最新角色是否可以触发回合开始/结束事件
    private static bool CanSettleRound(ActionUnit? actionUnit) => (actionUnit?.IsNormal ?? false) && actionUnit?.Actor != null;

    private void UpdateNewestAction()
    {
        if (_actionsByPriority[ActionPriority.FollowUpAttack].Count != 0)
        {
            _newestAction = _actionsByPriority[ActionPriority.FollowUpAttack].First();
            return;
        }

        if (_actionsByPriority[ActionPriority.Ultimate].Count != 0
            || _actionsByPriority[ActionPriority.ExtraTurn].Count != 0)
        {
            ActionUnit? ultimate = _actionsByPriority[ActionPriority.Ultimate].FirstOrDefault();
            ActionUnit? extra = _actionsByPriority[ActionPriority.ExtraTurn].FirstOrDefault();
            if (ultimate != null || extra != null)
            {
                if (extra == null)
                {
                    _newestAction = ultimate;
                }
                else if (ultimate == null)
                {
                    _newestAction = extra;
                }
                else
                {
                    _newestAction = ultimate.CreateId < extra.CreateId ? ultimate : extra;
                }
                return;
            }
        }

        if (_actionsByPriority[ActionPriority.NormalOperations].Count != 0)
        {
            _newestAction = _actionsByPriority[ActionPriority.NormalOperations].First();
        }
    }

    /// <summary>
    /// 从_actionsByPriority和_actionsByActorId中移除最新的行动
    /// </summary>
    private void RemoveFromPriId()
    {
        ActionUnit? actionUnit = NewestAction;
        if (actionUnit == null) return;
        _actionsByPriority[actionUnit.Priority].Remove(actionUnit);
        _actionsByActorId[actionUnit.OwnActor.Id]?.Remove(actionUnit.Priority);
    }

    private (IActor? actor, ISkill? skill) GetActorAndSkillFromNewestAction()
    {
        if (NewestAction == null) return (null, null);
        ActionUnit actionUnit = NewestAction!;
        IActor? actor = null;
        ISkill? actionSkill = null;
        switch (actionUnit.Priority)
        {
            case ActionPriority.FollowUpAttack:
                actor = actionUnit.OwnActor;
                actionSkill = actionUnit.Skill;
                break;
            case ActionPriority.ExtraTurn:
                actor = actionUnit.Actor;
                actionSkill = actionUnit.Skill;
                break;
            case ActionPriority.Ultimate:
                actor = actionUnit.OwnActor;
                actionSkill = actionUnit.Skill;
                break;
            case ActionPriority.NormalOperations:
                actor = actionUnit.Actor;
                actionSkill = actionUnit.Skill;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return (actor, actionSkill);
    }

    /// <summary>
    /// 归并排序
    /// </summary>
    private static List<ActionUnit> MergeByCreateId(List<ActionUnit> a, List<ActionUnit> b)
    {
        // 处理空列表的情况
        if (a.Count == 0)
            return b?.ToList() ?? [];
        if (b.Count == 0)
            return a.ToList();

        List<ActionUnit> result = [];
        int i = 0, j = 0;

        // 归并过程：逐个比较 CreateId
        while (i < a.Count && j < b.Count)
        {
            if (((IComparable)a[i].CreateId).CompareTo(b[j].CreateId) <= 0)
            {
                result.Add(a[i]);
                i++;
            }
            else
            {
                result.Add(b[j]);
                j++;
            }
        }

        // 添加剩余元素
        while (i < a.Count)
        {
            result.Add(a[i]);
            i++;
        }
        while (j < b.Count)
        {
            result.Add(b[j]);
            j++;
        }

        return result;
    }
    #endregion
}