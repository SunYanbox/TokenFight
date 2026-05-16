using TokenFight.Core.Constants;
using TokenFight.Core.Databases.Models.Dungeons;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Events.Contexts;
using TokenFight.Core.Models.Game;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Game.Dungeons;

/// <summary> 主战斗流程控制器 </summary>
[AutoDungeon(Id = GameIdTableConst.BattleFlow)]
public class BattleFlow : BaseDungeon
{
    private readonly GameSystemRegistry _systemRegistry;
    private readonly DungeonInfo? _dungeonInfo;
    private readonly Random _random = new();

    /// <summary> 主战斗流程控制器 </summary>
    public BattleFlow(GameSystemRegistry systemRegistry, string dungeonInfoId) : base(gameSystemRegistry: systemRegistry)
    {
        _systemRegistry = systemRegistry;
        _dungeonInfo = systemRegistry.DatabaseServer.DungeonInfoTables.GetValueOrDefault(dungeonInfoId);
        EnvironmentBuff!.Callbacks.Add(EventType.ActorDeath, HandleLoot);
    }

    protected void HandleLoot(IContext context)
    {
        if (context is not DeathContext { Actor: IEnemy }) return;
        if (Profile != null && _dungeonInfo is { PerRandomToken.Max: > 0 })
        {
            Profile.Token += _random.Next(_dungeonInfo.PerRandomToken.Min, _dungeonInfo.PerRandomToken.Max);
        }
    }

    public override void OnGameWin()
    {
        if (_systemRegistry.ActorManagerSystem.AllEnemies.Count != 0 ||
            _systemRegistry.ActorPoolSystem.EnemyCount != 0) return;
        Console.WriteLine("你赢了");
        EndTag = true;
        if (Profile != null && _dungeonInfo != null)
        {
            Profile.Token += _dungeonInfo.BaseToken;
            Console.WriteLine($"你获得{_dungeonInfo.BaseToken}个Token");
        }
    }

    public override void OnGameOver()
    {
        if (_systemRegistry.ActorManagerSystem.AllPlayers.Values.Sum(x => x.IsLive() ? 1 : 0) != 0 ||
            _systemRegistry.ActorPoolSystem.HasPlayer) return;
        Console.WriteLine("你输了");
        EndTag = true;
    }

    public override void InitActorPool()
    {
        if (_dungeonInfo == null) return;
        foreach (EnemyEntry enemyEntry in _dungeonInfo.EnemyPool)
        {
            _systemRegistry.ActorPoolSystem.AddEnemy(
                new Lazy<IActor>(() =>
                    _systemRegistry.ActorFactorySystem
                        .CreateInstance(enemyEntry.EnemyId, [enemyEntry.Level, _systemRegistry])));
        }
    }
}