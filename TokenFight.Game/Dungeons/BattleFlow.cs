using TokenFight.Core.Constants;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Entities;
using TokenFight.Core.Models.Game;
using TokenFight.Core.ReflectionAttribute;
using TokenFight.Game.Actors.Enemies;

namespace TokenFight.Game.Dungeons;

/// <summary> 主战斗流程控制器 </summary>
[AutoDungeon(Id=GameIdTableConst.BattleFlow)]
public class BattleFlow(GameSystemRegistry systemRegistry): BaseDungeon(gameSystemRegistry: systemRegistry)
{
    private readonly GameSystemRegistry _systemRegistry = systemRegistry;

    /// <summary>
    /// 加载完所有角色, 更新行动条前回调
    /// </summary>
    public override void OnEnterGame()
    {
        // foreach (var playerActor in ActorManagerSystem.AllPlayers.Values.OfType<PlayerActor>())
        // {
        //     playerActor.ActivateUltimateSkill();
        // }
    }
    
    public override void OnGameWin()
    {
        if (_systemRegistry.ActorManagerSystem.AllEnemies.Count != 0 ||
            _systemRegistry.ActorPoolSystem.EnemyCount != 0) return;
        Console.WriteLine("你赢了");
        EndTag = true;
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
        for (int i = 0; i < 50; i++)
        {
            int j = i;
            _systemRegistry .ActorPoolSystem.AddEnemy(
                new Lazy<IActor>(() => _systemRegistry.ActorFactorySystem.CreateInstance(GameIdTableConst.EnemyMuZhuang0,
                    [20, _systemRegistry])));
        }
    }
}