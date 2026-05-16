using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Entities.Masters;

public class RelationshipMaster(IActor owner) : IRelationshipMaster
{
    public WeakReference<IActor> Owner { get; set; } = new(owner);

    private static readonly Random Random = new();
    /// <summary> 队伍左侧目标 </summary>
    public WeakReference<IActor>? LeftActor { get; set; }
    /// <summary> 队伍右侧目标 </summary>
    public WeakReference<IActor>? RightActor { get; set; }
    /// <summary> 父成员 </summary>
    public WeakReference<IActor>? ParentActor { get; set; }
    /// <summary> 子成员 </summary>
    public Dictionary<string, WeakReference<IActor>>? ChildActors { get; set; }

    /// <summary> 获取主目标 </summary>
    public IEnumerable<IActor> GetSingleTargets() => Owner.TryGetTarget(out IActor? actor) ? [actor] : [];

    /// <summary> 获取主目标和相邻目标 </summary>
    public IEnumerable<IActor> GetDiffusionTargets()
    {
        List<IActor> data = GetSingleTargets().ToList();
        if (LeftActor?.TryGetTarget(out IActor? left) ?? false)
            data.Add(left);
        if (RightActor?.TryGetTarget(out IActor? right) ?? false)
            data.Add(right);
        return data;
    }
    /// <summary> 获取全队成员 </summary>
    public IEnumerable<IActor> GetAllTargets()
    {
        List<IActor> data = GetSingleTargets().ToList();
        IActor? left = ActorHelper.GetActorFromWeakRef(LeftActor);
        int deep = 8;
        while (left != null && deep > 0)
        {
            data.Add(left);
            left = ActorHelper.GetActorFromWeakRef(LeftActor);
            deep--;
        }
        IActor? right = ActorHelper.GetActorFromWeakRef(RightActor);
        deep = 8;
        while (right != null && deep > 0)
        {
            data.Add(right);
            right = ActorHelper.GetActorFromWeakRef(RightActor);
            deep--;
        }
        return data;
    }
    /// <summary> 获取指定数量的可放回目标 </summary>
    /// <param name="count"></param>
    public IEnumerable<IActor> GetRandomTargets(int count = 1)
    {
        IActor[] targets = GetSingleTargets().ToArray();
        List<IActor> sample = [];
        if (targets.Length == 0) return sample;

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Next(targets.Length);
            sample.Add(targets[randomIndex]);
        }

        return sample;
    }
    /// <summary> 清理引用 </summary>
    public void ClearRef()
    {
        if (ChildActors != null)
        {
            foreach (WeakReference<IActor> weakReferenceActor in ChildActors.Values.ToArray())
            {
                if (weakReferenceActor.TryGetTarget(out IActor? actor))
                    actor.OnDeath();
            }
        }
        LeftActor = null;
        RightActor = null;
        ParentActor = null;
        ChildActors?.Clear();
    }
}
