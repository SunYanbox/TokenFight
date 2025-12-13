using TokenFight.Core.Interfaces.Bases;
using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Interfaces.Systems;

public interface IActionListSystem: ISystem
{
    /// <summary> 获取行动序列 </summary>
    public IReadOnlyList<IActor> ActionList { get; }

    /// <summary> 将成员添加到行动队列末尾 </summary>
    public void Append(IActor actor);

    /// <summary> 最快行动的角色 </summary>
    public IActor? FastestActor { get; }

    /// <summary> 令所有角色按照行动值最低的行动值进行行动 </summary>
    public void ActionAll();

    /// <summary> 移除指定的角色 </summary>
    public void Remove(IActor actor);

    /// <summary> 排序行动队列（必须是稳定排序！）</summary>
    public void SortActionList();

    /// <summary> 令所有角色按照行动值最低的行动值进行行动, 随后排序行动队列 </summary>
    public void ActionAndSort()
    {
        ActionAll();
        SortActionList();
    }
}