using System.Text;
using TokenFight.Core.Consoles.Display;
using TokenFight.Core.Enums.Actions;
using TokenFight.Core.Interfaces.Systems;
using TokenFight.Core.Models.Actions;

namespace TokenFight.Core.Consoles.Details;

public static class ToDetailStringUtil
{
    public static string DumpsActionListSystem(IActionListSystem actionListSystem)
    {
        IEnumerable<string> items = actionListSystem.ActionList.Select(s =>
            $"{s.Name}({s.Id}~{DataShowUtil.ShowActionValue(s.ActionValueMaster.ActionValue)})");
        return $"行动队列: [ {string.Join(", ", items)} ] ";
    }

    public static string DumpsActionUnit(ActionUnit actionUnit)
    {
        var msg = new StringBuilder();
        msg.Append("ActionUnit( ");
        if (actionUnit.IsNormal) { msg.Append($"普通行动-{actionUnit.Actor?.Name} {actionUnit.Skill?.Name}"); }
        if (actionUnit.IsExtraTurn) { msg.Append($"额外回合-{actionUnit.Actor?.Name} {actionUnit.Skill?.Name}"); }
        if (actionUnit.IsUltimate) { msg.Append($"终结技-{actionUnit.Skill?.Name}"); }
        if (actionUnit.IsFollowUpAttack) { msg.Append($"追加攻击-{actionUnit.Skill?.Name}"); }
        msg.Append(" )");
        return msg.ToString();
    }

    public static string DumpsActionManagerSystem(IActionManagerSystem actionManagerSystem)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("ActionManagerSystem( ");
        stringBuilder.AppendJoin(", ", actionManagerSystem.GetActionList().Select(DumpsActionUnit));
        stringBuilder.Append(" )");
        return stringBuilder.ToString();
    }

    public static void OutputActionManagerSystem(IActionManagerSystem actionManagerSystem)
    {
        var printer = new ConsolePrinter();
        printer.Add("行动队列: ");
        foreach (ActionUnit actionUnit in actionManagerSystem.GetActionList())
        {
            switch (actionUnit.Priority)
            {
                case ActionPriority.FollowUpAttack:
                    printer.Add($"追加攻击-{actionUnit.Skill?.Name} ", ConsoleColor.Blue);
                    break;
                case ActionPriority.ExtraTurn:
                    printer.Add($"额外回合-{actionUnit.Actor?.Name} {actionUnit.Skill?.Name} ", ConsoleColor.Cyan);
                    break;
                case ActionPriority.Ultimate:
                    printer.Add($"终结技-{actionUnit.Skill?.Name} ", ConsoleColor.Magenta);
                    break;
                case ActionPriority.NormalOperations:
                    printer.Add($"普通行动-{actionUnit.Actor?.Name} {actionUnit.Skill?.Name} ", ConsoleColor.White);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        printer.Add("\n");
        printer.Display();
    }
}