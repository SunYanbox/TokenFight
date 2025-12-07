using TokenFight.Core.Enums.Actions;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Models.Actions;

/// <summary> 一个行动的单位 </summary>
public record ActionUnit
{
    /// <summary> 获取这次行动所属的角色 </summary>
    public IActor OwnActor => Actor ?? Skill!.Source;

    /// <summary> 创建索引, 用来确定同一类行动的顺序 </summary>
    public long CreateId { get; init; }

    /// <summary> 普通行动或额外行动的角色 </summary>
    public IActor? Actor { get; set; }

    /// <summary> 普通行动选择的技能 </summary>
    public ISkill? Skill { get; set; }

    /// <summary> 该行动的优先级 </summary>
    public ActionPriority Priority { get; set; }

    /// <summary> 是否行动完毕 </summary>
    public bool ActionEnd { get; set; }

    /// <summary> 是否为追加攻击 </summary>
    public bool IsFollowUpAttack => Priority == ActionPriority.FollowUpAttack;

    /// <summary> 是否为额外回合 </summary>
    public bool IsExtraTurn => Priority == ActionPriority.ExtraTurn;

    /// <summary> 是否为终结技 </summary>
    public bool IsUltimate => Priority == ActionPriority.Ultimate;

    /// <summary> 是否为普通行动 </summary>
    public bool IsNormal => Priority == ActionPriority.NormalOperations;

    // 使用固定字段计算哈希码
    private readonly int _id = Guid.NewGuid().GetHashCode();

    /// <summary> 是否相等 </summary>
    public virtual bool Equals(ActionUnit? other) => other is not null && _id == other._id;

    /// <summary> 获取哈希值 </summary>
    public override int GetHashCode() => _id;
}