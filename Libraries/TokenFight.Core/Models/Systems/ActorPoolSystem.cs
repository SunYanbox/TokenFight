using TokenFight.Core.Constants;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Systems;

namespace TokenFight.Core.Models.Systems;

public class ActorPoolSystem(IActorManagerSystem actorManagerSystem): IActorPoolSystem
{
    private readonly Queue<Lazy<IActor>> _playerPool = new();
    private readonly List<Queue<Lazy<IActor>>> _enemyPool = [];
    public bool HasPlayer => _playerPool.Count > 0;
    public bool IsEmptyWave => _enemyPool[Wave].Count > 0;
    /// <summary> 当前波次剩余敌人数量 </summary>
    public int EnemyCurrentCount => _enemyPool[Wave].Count;
    /// <summary> 剩余玩家数量 </summary>
    public int PlayerCount => _playerPool.Count;
    /// <summary> 剩余敌人数量 </summary>
    public int EnemyCount => _enemyPool.Select(x => x.Count).Sum();
    /// <summary> 最大敌人数量 </summary>
    public int EnemyMaxCount { get; set; } = 0;
    /// <summary> 当前波次 </summary>
    public int Wave { get; set; } = 0;
    /// <summary> 最大波次 </summary>
    public int WaveMax { get; set; } = 1;
    
    public void Init()
    {
        _enemyPool.Add(new Queue<Lazy<IActor>>());
    }

    public void Reset()
    {
        _playerPool.Clear();
        _enemyPool.Clear();
        Init();
        Wave = 0;
        WaveMax = 1;
        EnemyMaxCount = 0;
    }

    public void FillActors()
    {
        while (HasPlayer && actorManagerSystem.AllPlayers.Values.Count < GameConst.PlayerInFieldLimit)
        {
            ActorHelper.CreateActorToField(PlayerNext().Value);
        }
        while (EnemyCount > 0 && actorManagerSystem.AllEnemies.Values.Count < GameConst.EnemyInFieldLimit)
        {
            ActorHelper.CreateActorToField(EnemyNext().Value);
        }
    }

    public void AddPlayer(Lazy<IActor> Actor)
    {
        _playerPool.Enqueue(Actor);
    }

    public void AddEnemy(Lazy<IActor> Actor)
    {
        if (_enemyPool[WaveMax-1].Count >= GameConst.EnemyWaveCountLimit)
        {
            AddNewWave();
        }
        _enemyPool[WaveMax-1].Enqueue(Actor);
        UpdateWareAndCoundInfo();
    }

    public Lazy<IActor> PlayerNext() => _playerPool.Dequeue();

    public void EnterNextWave()
    {
        Wave++;
        if (Wave == WaveMax) Wave = WaveMax - 1;
    }
    
    public Lazy<IActor> EnemyNext()
    {
        if (EnemyCurrentCount <= 0) EnterNextWave();
        return _enemyPool[Wave].Dequeue();
    }
    public void AddNewWave()
    {
        _enemyPool.Add(new Queue<Lazy<IActor>>());
        UpdateWareAndCoundInfo();
    }
    
    private void UpdateWareAndCoundInfo()
    {
        WaveMax = Math.Max(WaveMax, _enemyPool.Count);
        EnemyMaxCount = Math.Max(EnemyMaxCount, EnemyCount);
    }
}