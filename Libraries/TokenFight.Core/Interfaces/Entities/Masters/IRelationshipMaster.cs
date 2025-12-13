namespace TokenFight.Core.Interfaces.Entities.Masters;

public interface IRelationshipMaster: IMaster
{
    /// <summary> 队伍左侧目标 </summary>
    public WeakReference<IActor>? LeftActor { get; set; }
    /// <summary> 队伍右侧目标 </summary>
    public WeakReference<IActor>? RightActor { get; set; }
    /// <summary> 父成员 </summary>
    public WeakReference<IActor>? ParentActor { get; set; }
    /// <summary> 子成员 </summary>
    public Dictionary<string, WeakReference<IActor>>? ChildActors { get; set; }

}