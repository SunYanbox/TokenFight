using TokenFight.Core.Interfaces.Effects;

namespace TokenFight.Core.Interfaces.Entities;

public interface IPlayer
{
    /// <summary> 是否拥有终结技 </summary>
    public bool HasUltimateSkill();

    /// <summary> 是否拥有追加攻击 </summary>
    public bool HasFollowUpAttack();

    /// <summary> 获取战技技能 </summary>
    public ISkill GetFightSkill();

    /// <summary> 获取终结技技能 </summary>
    public ISkill GetUltimateSkill();

    /// <summary> 获取追加攻击技能 </summary>
    public ISkill GetFollowUpAttack();

    /// <summary> 激活终结技 </summary>
    public void ActivateUltimateSkill();
}