using TokenFight.Core.Consoles;
using TokenFight.Core.Databases.Models.Profiles;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Game;
using TokenFight.Core.Models.Actions;
using TokenFight.Core.Models.Controls;
using TokenFight.Core.Models.Effects;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Models.Game;

[AutoDungeon(Id = "BaseDungeon")]
public class BaseDungeon(GameSystemRegistry gameSystemRegistry) : IDungeon
{
    #region 成员实例
    public double? ActionValueLimit { get; set; }
    public required PassiveData EnvironmentBuff { get; set; } = new(gameSystemRegistry.EventSystem, gameSystemRegistry.LocalLog);
    /// <summary> 关联的存档 </summary>
    public Profile? Profile { get; protected set; }
    /// <summary> 游戏是否结束 </summary>
    public bool EndTag { get; protected set; }

    /// <summary> 玩家行动代理 </summary>
    public readonly AgentPlayer AgentPlayer = new(gameSystemRegistry.LocalLog, gameSystemRegistry.ActionManagerSystem, gameSystemRegistry.ActorManagerSystem, gameSystemRegistry.ActorPositionSystem);
    /// <summary> 敌人行动代理 </summary>
    public readonly AgentEnemy AgentEnemy = new();
    #endregion

    /// <summary> 清理所有战斗相关数据 </summary>
    public virtual void Clear()
    {
        gameSystemRegistry.ActorManagerSystem.Reset();
        gameSystemRegistry.ActorPoolSystem.Reset();
        gameSystemRegistry.ActionManagerSystem.Reset();
        gameSystemRegistry.ActorPositionSystem.Reset();
        gameSystemRegistry.ActionListSystem.Reset();
        gameSystemRegistry.GlobalResourcesSystem.Reset();
        EndTag = false;
    }

    /// <summary>
    /// 加载完所有角色, 更新行动条前回调
    /// </summary>
    public virtual void OnEnterGame()
    {
        EnvironmentBuff.Subscribe();
    }

    public virtual void OnGameWin()
    {
        if (gameSystemRegistry.ActorManagerSystem.AllEnemies.Count == 0 && gameSystemRegistry.ActorPoolSystem.EnemyCount == 0)
        {
            Console.WriteLine("你赢了");
            EndTag = true;
        }
    }

    public virtual void OnGameOver()
    {
        if (gameSystemRegistry.ActorManagerSystem.AllPlayers.Values.Sum(x => x.IsLive() ? 1 : 0) == 0 && !gameSystemRegistry.ActorPoolSystem.HasPlayer)
        {
            Console.WriteLine("你输了");
            EndTag = true;
        }
    }

    public virtual void OnLoad(Profile profile)
    {
        Clear();
        Profile = profile;
    }

    public virtual void InitActorPool()
    {

    }

    public virtual void OnUpdate()
    {

    }

    public virtual void Main()
    {
        gameSystemRegistry.ActorPoolSystem.FillActors();
        OnEnterGame();
        while (!EndTag)
        {
            gameSystemRegistry.ActorPoolSystem.FillActors();
            gameSystemRegistry.ActionListSystem.ActionAndSort();
            gameSystemRegistry.ActorPositionSystem.UpdateActorRelationship(gameSystemRegistry.ActorManagerSystem);
            if (gameSystemRegistry.ActionListSystem.FastestActor == null)
            {
                Console.WriteLine("> actionListSystem.FastestActor为null");
                EndTag = true;
                continue;
            }
            else
            {
                if (gameSystemRegistry.ActionManagerSystem.Empty)
                {
                    CreateActionHelper.CreateNormal(gameSystemRegistry.ActionListSystem.FastestActor);
                }
            }
            if (ControlConsole.CheckAndHandleUltimates("回合开始前")) continue;
            gameSystemRegistry.ActionManagerSystem.RoundBegin();
            DataShowUtil.ShowGameField(gameSystemRegistry);

            ActionUnit? newestAction = gameSystemRegistry.ActionManagerSystem.NewestAction;

            if (newestAction != null)
            {
                IActor actor = newestAction.OwnActor;
                if (AgentEnemy.HandleSkillChoice(actor, newestAction)) continue;
                if (AgentPlayer.HandleSkillChoice(actor, newestAction)) continue;


                (IActor? actor1, ISkill? skill) = gameSystemRegistry.ActionManagerSystem.ActionBegin();

                if (actor1 != null && skill != null)
                {
                    EventHelper.TriggerReleaseSkillContext(actor1, skill);
                    do
                    {
                        AgentEnemy.HandleTargetChoice(actor1, newestAction);
                        AgentPlayer.HandleTargetChoice(actor1, newestAction);
                        skill.Execute();
                        gameSystemRegistry.ActorPositionSystem.UpdateActorRelationship(gameSystemRegistry.ActorManagerSystem);
                    } while (skill.KeepAction());
                }
            }

            gameSystemRegistry.ActionManagerSystem.ActionEnd();
            if (ControlConsole.CheckAndHandleUltimates("行动结束时")) continue;
            gameSystemRegistry.ActionManagerSystem.RoundEnd();
            if (ControlConsole.CheckAndHandleUltimates("回合结束时")) continue;

            Thread.Sleep(750);
            OnGameWin();
            OnGameOver();
            gameSystemRegistry.ActorPoolSystem.FillActors();
        }

    }

    public virtual void OnDestroy()
    {
        EnvironmentBuff.Unsubscribe();
        Clear();
    }
}