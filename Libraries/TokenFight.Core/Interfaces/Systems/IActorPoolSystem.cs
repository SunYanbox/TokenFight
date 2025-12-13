using TokenFight.Core.Interfaces.Bases;
using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Interfaces.Systems;

/// <summary> 角色池系统 </summary>
public interface IActorPoolSystem: ISystem
{
    /// <summary> 向对局中填充成员 </summary>
    public void FillActors();

    /// <summary> 添加新的成员到池子中 </summary>
    public void AddPlayer(Lazy<IActor> actor);

    /// <summary> 添加新的成员到池子中 </summary>
    public void AddEnemy(Lazy<IActor> actor);

    /// <summary> 获取下一个玩家 </summary>
    public Lazy<IActor> PlayerNext();

    /// <summary> 获取下一个敌人 </summary>
    public Lazy<IActor> EnemyNext();

    /// <summary> 判断是否有任何玩家 </summary>
    public bool HasPlayer { get; }
    /// <summary> 判断当前波次是否有任何敌人 </summary>
    public bool IsEmptyWave { get; }
    /// <summary> 进入下一个波次 </summary>
    public void EnterNextWave();
    /// <summary> 为敌人添加新的波次 </summary>
    public void AddNewWave();

    /// <summary> 当前波次剩余敌人数量 </summary>
    public int EnemyCurrentCount { get; }
    /// <summary> 剩余玩家数量 </summary>
    public int PlayerCount { get; }
    /// <summary> 剩余敌人数量 </summary>
    public int EnemyCount { get; }
    /// <summary> 最大敌人数量 </summary>
    public int EnemyMaxCount { get; protected set; }
    /// <summary> 当前波次 </summary>
    public int Wave { get; protected set; }
    /// <summary> 最大波次 </summary>
    public int WaveMax { get; protected set; }
}