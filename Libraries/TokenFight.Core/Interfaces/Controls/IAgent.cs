using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models.Actions;

namespace TokenFight.Core.Interfaces.Controls;

/// <summary> 代理角色行动, 在控制台显示技能选择信息的接口 </summary>
public interface IAgent
{
    /// <summary> 表示一种伪随机数生成器，该算法可生成满足特定随机性统计要求的数列  </summary>
    protected Random Random { get; init; }

    /// <summary> 判断当前类是否可以处理传入的成员派生类 </summary>
    public bool CanHandle(IActor actor);

    /// <summary>
    /// 处理actor技能选择的行动
    /// <br />
    /// 并将选择结果赋值到actionUnit中
    /// </summary>
    /// <returns>是否是刚选择释放终结技</returns>
    public bool HandleSkillChoice(IActor actor, ActionUnit actionUnit);

    /// <summary>
    /// 处理actor目标选择的行动
    /// <br />
    /// 并将选择结果赋值到actionUnit.Skill中
    /// </summary>
    public void HandleTargetChoice(IActor actor, ActionUnit actionUnit);


    protected static void OutputTalentData(IActor actor)
    {
        List<ISkill> cantChoiceSkill = [];
        cantChoiceSkill.AddRange(actor.SkillMaster.GetSkillAll().Where(x => !x.CanUse()));
        foreach (ISkill x in cantChoiceSkill)
        {
            string prefix = x.Type == SkillType.NaturalTalent ? "被动" : "不可用";
            Console.WriteLine($"[{prefix}] {x.Name} {x.Type.ToString()} {x.Desc}");
        }
    }
}
