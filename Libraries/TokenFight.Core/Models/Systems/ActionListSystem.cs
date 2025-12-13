using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Systems;

namespace TokenFight.Core.Models.Systems;

public class ActionListSystem(IGlobalResourcesSystem globalResourcesSystem): IActionListSystem
{
    private class ActionSlot
    {
        public required IActor Actor;
        public int Sequence; // 全局递增，保证稳定排序
    }

    private readonly List<ActionSlot> _actionSlots = [];
    private int _nextSequence = 0;

    public void Init() { }

    public void Reset()
    {
        _actionSlots.Clear();
        _nextSequence = 0;
    }

    public IReadOnlyList<IActor> ActionList =>
        _actionSlots.Select(s => s.Actor).ToList().AsReadOnly();

    public void Append(IActor actor)
    {
        if (_actionSlots.Any(s => s.Actor == actor)) return;

        actor.ActionValueMaster.Reset();
        _actionSlots.Add(new ActionSlot
        {
            Actor = actor,
            Sequence = _nextSequence++
        });
        SortActionList();
    }

    public IActor? FastestActor =>
        _actionSlots.Count > 0 ? _actionSlots[0].Actor : null;
    public void ActionAll()
    {
        if (_actionSlots.Count == 0) return;

        double minActionValue = _actionSlots
            .Min(s => s.Actor.ActionValueMaster.ActionValue);

        if (minActionValue >= double.Epsilon)
        {
            foreach (ActionSlot slot in _actionSlots)
            {
                slot.Actor.ActionValueMaster.Action(minActionValue);
            }
            globalResourcesSystem.AdjustActionValue(minActionValue);
        }
    }

    public void Remove(IActor actor)
    {
        _actionSlots.RemoveAll(s => s.Actor == actor);
    }

    public void SortActionList()
    {
        if (_actionSlots.Count <= 1) return;

        // 使用快照避免动态值干扰 + 稳定排序
        var snapshot = _actionSlots.Select(s => new
        {
            Slot = s,
            ActionValue = s.Actor.ActionValueMaster?.ActionValue ?? 0
        }).ToList();

        // 先按 ActionValue 升序，再按 Sequence 升序（先加入的在前）
        snapshot.Sort((a, b) =>
        {
            int cmp = a.ActionValue.CompareTo(b.ActionValue);
            if (cmp != 0) return cmp;
            return a.Slot.Sequence.CompareTo(b.Slot.Sequence);
        });

        // 回写顺序
        for (int i = 0; i < snapshot.Count; i++)
        {
            _actionSlots[i] = snapshot[i].Slot;
        }
    }


}