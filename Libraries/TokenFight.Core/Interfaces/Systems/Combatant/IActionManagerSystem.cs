using TokenFight.Core.Enums.Actions;
using TokenFight.Core.Interfaces.Bases;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models.Actions;

namespace TokenFight.Core.Interfaces.Systems.Combatant;

/// <summary>
/// 管理游戏中所有行动单位的调度、生命周期与执行顺序的核心系统。
/// 负责区分不同类型的行动（普通、终结技、追加攻击、额外回合等），
/// 并维护已执行行动与已处理回合开始事件的历史记录。
/// </summary>
public interface IActionManagerSystem : ISystem
{
    /// <summary>
    /// 创建一个行动
    /// 额外回合和普通回合赋值Actor
    /// 追加攻击和终结技赋值skill
    /// </summary>
    public bool CreateAction(ActionPriority priority, IActor? actor = null, ISkill? skill = null);

    /// <summary> 当前行动队列是否为空 </summary>
    public bool Empty { get; }

    /// <summary> 获取最新行动单位 </summary>
    public ActionUnit? NewestAction { get; }

    /// <summary> 获取所有优先行动队列和普通行动 </summary>
    public List<ActionUnit> GetActionList();

    #region 行动相关逻辑
    /// <summary>
    /// 使得首个角色回合开始
    /// </summary>
    public void RoundBegin();

    /// <summary>
    /// 使得所有行动过的角色回合结束, 随后发布回合结束事件
    /// </summary>
    public void RoundEnd();

    /// <summary>
    /// 使得最新的行动开始, 重置普通行动角色的行动值, 发布行动开始事件
    ///
    /// 行动开始后只有NewestAction暂时持有最新行动的引用(普通行动会在_handledRoundStart中暂时存在)
    /// </summary>
    /// <returns>返回(当前行动的角色, 当前选择的技能)构成的元组</returns>
    public (IActor? actor, ISkill? skill) ActionBegin();

    /// <summary>
    /// 行动结束, 清理最新成员属性
    /// </summary>
    public void ActionEnd();
    #endregion
}