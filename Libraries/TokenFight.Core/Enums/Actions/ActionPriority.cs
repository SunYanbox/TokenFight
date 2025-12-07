namespace TokenFight.Core.Enums.Actions;

/// <summary> 行动的优先级 </summary>
public enum ActionPriority
{
    /// <summary> 追加攻击 </summary>
    FollowUpAttack,
    /// <summary> 额外回合 </summary>
    ExtraTurn,    
    /// <summary> 终结技 </summary>
    Ultimate,
    /// <summary> 普通行动 </summary>
    NormalOperations
}