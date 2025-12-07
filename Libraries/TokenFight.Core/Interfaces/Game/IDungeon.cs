using TokenFight.Core.Databases.Models.Profiles;
using TokenFight.Core.Models.Effects;

namespace TokenFight.Core.Interfaces.Game;

/// <summary> 副本设置 </summary>
public interface IDungeon
{
    /// <summary> 对局行动值限制 </summary>
    public double? ActionValueLimit { get; set; }
    /// <summary> 环境紊流 </summary>
    public PassiveData EnvironmentBuff { get; set; }
    /// <summary> 游戏胜利 </summary>
    public void OnGameWin();
    /// <summary> 游戏失败 </summary>
    public void OnGameOver();
    /// <summary> 进入副本回调 </summary>
    public void OnLoad(Profile profile);
    /// <summary> 初始化角色池 </summary>
    public void InitActorPool();
    /// <summary> 更新回调 </summary>
    public void OnUpdate();
    /// <summary> 启动游戏循环 </summary>
    public void Main();
    /// <summary> 退出副本回调 </summary>
    public void OnDestroy();
}