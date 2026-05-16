using TokenFight.Core.Enums.Actions;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Effects.Skills;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Consoles;

/// <summary> 控制台控制 </summary>
[AutoSysRegistryInit]
public static class ControlConsole
{
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }

    /// <summary>
    /// 轮询可用的终结技,
    /// 如果玩家放弃释放终结技或者没有可用的终结技, 则会返回false;
    /// </summary>
    /// <returns>是否需要跳过当前回合的其他操作(创造了优先行动)</returns>
    public static bool CheckAndHandleUltimates(string title = "")
    {
        List<IUltimateSkill> readyList = ActorHelper.GetAllUltimateSkillCanUse();
        if (readyList.Count == 0) return false; // 无大招，继续

        if (!string.IsNullOrEmpty(title))
            Console.Write($"\n{title}:");
        var choiceSkill =
            SelectionUtil.SelectFromList<ISkill>(readyList, x => $"{x.Name} {x.Desc}", allowQuit: true);

        if (choiceSkill is BaseUltimateSkill baseUltimateSkill)
        {
            baseUltimateSkill.IsUsing = true;
            GameSystemRegistry?.ActionManagerSystem.CreateAction(ActionPriority.Ultimate, skill: baseUltimateSkill);
            // 释放终结技后退出
            return true;
        }

        return false; // 用户选择跳过，继续
    }
}
