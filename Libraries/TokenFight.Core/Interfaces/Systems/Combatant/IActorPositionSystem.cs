using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Interfaces.Bases;

namespace TokenFight.Core.Interfaces.Systems.Combatant;

/// <summary> 管理所有角色站位的系统 </summary>
public interface IActorPositionSystem : ISystem
{
    /// <summary> 玩家角色数量 </summary>
    public int PlayerCount { get; }
    /// <summary> 敌人角色数量 </summary>
    public int EnemyCount { get; }
    /// <summary> 玩家角色 </summary>
    public List<string> PlayerPosition { get; }
    /// <summary> 敌人角色 </summary>
    public List<string> EnemyPosition { get; }

    /// <summary> 将一个成员添加到站位末尾 </summary>
    public void Append(string id, TeamType team);

    /// <summary> 将一个成员添加到给定索引位置 </summary>
    public void Insert(int position, string id, TeamType team);

    /// <summary> 获取指定成员的位置 </summary>
    public int GetPosition(string id, TeamType team);

    /// <summary> 移除指定成员的位置 </summary>
    public void Remove(string id, TeamType team);

    /// <summary> 根据站位信息更新成员的左右关系 </summary>
    public void UpdateActorRelationship(IActorManagerSystem actorManagerSystem);
}
