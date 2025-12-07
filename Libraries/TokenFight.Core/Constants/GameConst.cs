namespace TokenFight.Core.Constants;

/// <summary>
/// 游戏中一些固定的常量
/// </summary>
public static class GameConst
{
    #region 战斗相关常量
    /// <summary> 默认行动距离 </summary>
    public const double DefaultActionDistance = 10000.00;
    /// <summary> 防御区常数-斜率 </summary>
    public const double DefenseK = 20;
    /// <summary> 防御区常数-截距 </summary>
    public const double DefenseB = 200;
    /// <summary> 玩家一次上场的成员数量限制 </summary>
    public const int PlayerInFieldLimit = 8;
    /// <summary> 敌人一次上场的成员数量限制 </summary>
    public const int EnemyInFieldLimit = 5;
    /// <summary> 一个波次的最大敌人数量限制 </summary>
    public const int EnemyWaveCountLimit = 15;
    /// <summary> 所有单位行动次数上限 </summary>
    public const int ActionTotalLimit = 100_0000;
    #endregion

    #region 控制台输出的符号
    /// <summary> 允许显示的最大行动值 </summary>
    public const int ActionValueMaxValue = 999999;
    /// <summary> 额外回合的标记 </summary>
    public const string SignExtraTurn = "+";
    /// <summary> 终结技的标记 </summary>
    public const string SignUltimate = "!";
    /// <summary> 追加攻击的标记 </summary>
    public const string SignFollowUpAttack = ">";
    #endregion

    #region 游戏配置相关常量
    /// <summary> 日志文件夹路径 </summary>
    public const string LogFolder = "logs";
    /// <summary> 数据文件夹路径 </summary>
    public const string DataFolder = "data";
    #endregion
}