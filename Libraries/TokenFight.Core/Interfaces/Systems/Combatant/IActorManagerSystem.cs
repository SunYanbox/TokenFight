using TokenFight.Core.Interfaces.Bases;
using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Interfaces.Systems.Combatant;

/// <summary> 管理所有成员的系统 </summary>
public interface IActorManagerSystem: ISystem
{
    /// <summary> 获取所有注册过的成员的引用 </summary>
    public Dictionary<string, IActor> AllActors { get; }
    /// <summary> 获取所有注册过的玩家成员的引用 </summary>
    public Dictionary<string, IActor> AllPlayers { get; }
    /// <summary> 获取所有注册过的敌人成员的引用 </summary>
    public Dictionary<string, IActor> AllEnemies { get; }
}