using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Effects;

namespace TokenFight.Core.Interfaces.Entities.Masters;

/// <summary> 技能管理器 </summary>
public interface ISkillMaster : IMaster
{
    /// <summary> 判断技能是否包含指定键的技能 </summary>
    public bool ContainsKey(string id);

    /// <summary> 清理所有回调 </summary>
    public void UnsubscriptAll();

    /// <summary> 设置技能组目标, 并返回技能系统自身 </summary>
    public ISkillMaster SetTarget(IActor target);

    /// <summary> 增加一个技能 </summary>
    public void Add(ISkill skill);

    /// <summary> 移除指定Id的技能 </summary>
    public void Remove(string id);

    /// <summary> 通过Id获取指定技能实例 </summary>
    public ISkill GetSkill(string id);

    /// <summary> 判断是否存在指定Id的技能 </summary>
    public bool HasSkill(string id);

    /// <summary> 获取所有可用的主动技能 </summary>
    public IEnumerable<ISkill> GetActiveSkills();

    /// <summary> 获取所有可用的主动技能 除了终结技 </summary>
    public IEnumerable<ISkill> GetActiveSkillsApartFromUltimate();

    /// <summary> 获取所有可用的被动技能 </summary>
    public IEnumerable<ISkill> GetPassiveSkills();

    /// <summary> 获取所有技能 </summary>
    public IEnumerable<ISkill> GetSkillAll();

    protected static void SetSkillTarget(ISkill skill, IActor? target)
    {
        skill.Target ??= new WeakReference<IActor>(null!);
        switch (skill.Choice)
        {
            case SkillChoiceType.OnlySelf:
                skill.Target.SetTarget(skill.Source);
                break;
            case SkillChoiceType.OnlyEnemy:
                if (skill.Source.Team == target?.Team) break;
                skill.Target.SetTarget(skill.Source);
                break;
            case SkillChoiceType.AnyAllies:
                if (skill.Source.Team != target?.Team) break;
                skill.Target.SetTarget(skill.Source);
                break;
            case SkillChoiceType.OnlyAllies:
                if (skill.Source.Team != target?.Team && skill.Source.Id != target?.Id) break;
                skill.Target.SetTarget(skill.Source);
                break;
        }
    }

}
