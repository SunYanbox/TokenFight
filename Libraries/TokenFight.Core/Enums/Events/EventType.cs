namespace TokenFight.Core.Enums.Events;

/// <summary>
/// 游戏内事件的类型
/// </summary>
public enum EventType
{
    /// <summary> 空事件 </summary>
    Null,
    
    /// <summary> 游戏开始 </summary>
    GameStart,
    /// <summary> 游戏结束 </summary>
    GameEnd,
    
    /// <summary> 成员生命周期 </summary>
    ActorLife,
    /// <summary> 成员死亡 </summary>
    ActorDeath,
    
    /// <summary> 血量发生变化 </summary>
    HealthChange,
    /// <summary> 进入对局时触发 </summary>
    EnterGame,
    /// <summary> 行动开始 </summary>
    ActionStart,
    /// <summary> 行动结束 </summary>
    ActionEnd,
    /// <summary> 回合开始 </summary>
    RoundBegin,
    /// <summary> 回合结束 </summary>
    RoundEnd,
    /// <summary> 释放技能 </summary>
    ReleaseSkill,

    #region 主要操作事件
    /// <summary> 计算属性前 </summary>
    CalculateBefore,
    /// <summary> 应用效果 </summary>
    EffectApply,
    /// <summary> 移除效果 </summary>
    EffectRemove,
    /// <summary> 造成伤害前 </summary>
    DamageBefore,
    /// <summary> 造成伤害 </summary>
    Damage,
    /// <summary> 提供治疗前 </summary>
    HealBefore,
    /// <summary> 提供治疗 </summary>
    Heal,
    /// <summary> 提供护盾前 </summary>
    ShieldBefore,
    /// <summary> 提供护盾 </summary>
    Shield,
    /// <summary> 造成推条前 </summary>
    PushBefore,
    /// <summary> 推条 </summary>
    Push
    #endregion
}