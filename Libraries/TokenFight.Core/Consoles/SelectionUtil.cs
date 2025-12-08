using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Consoles;

/// <summary>
/// 提供控制台列表选择的通用工具。
/// </summary>
public static class SelectionUtil
{
    /// <summary>
    /// 获取技能可选择的目标队列
    /// </summary>
    public static IActor[] GetActorWhereSkillChoice(ISkill skill)
    {
        switch (skill.Choice)
        {
            case SkillChoiceType.OnlySelf:
                return [skill.Source];
            case SkillChoiceType.OnlyAllies:
                return ActorHelper.GetRelativeAllies(skill.Source, true);
            case SkillChoiceType.AnyAllies:
                return ActorHelper.GetRelativeAllies(skill.Source, false);
            case SkillChoiceType.OnlyEnemy:
                return ActorHelper.GetRelativeEnemies(skill.Source);
            default:
                throw new ArgumentOutOfRangeException(nameof(SkillChoiceType));
        }
    }
    
    /// <summary>
    /// 显示选项列表并等待用户选择。
    /// </summary>
    /// <typeparam name="T">选项类型</typeparam>
    /// <param name="options">选项列表</param>
    /// <param name="displayFormatter">将选项格式化为显示字符串的函数，推荐格式："名称, 描述"</param>
    /// <param name="prompt">提示文本（默认："请选择: "）</param>
    /// <param name="allowQuit">是否允许通过 'q'/'quit'/'exit' 退出（默认：false）</param>
    /// <param name="quitValue">退出时返回的值（默认：default(T)）</param>
    /// <returns>用户选择的选项，或 quitValue（若允许退出且用户选择退出）</returns>
    public static T? SelectFromList<T>(
        IReadOnlyList<T> options,
        Func<T, string> displayFormatter,
        string prompt = "请选择: ",
        bool allowQuit = false,
        T? quitValue = default)
    {
        if (options == null || options.Count == 0)
            throw new ArgumentException("选项列表不能为空", nameof(options));

        while (true)
        {
            // 显示选项
            Console.WriteLine();
            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"[{i+1}] " + displayFormatter(options[i]));
            }

            if (allowQuit)
            {
                Console.WriteLine($"输入 {GetQuitWords()} 退出");
            }
            Console.Write($"{prompt} ");

            // 读取输入
            string? input = Console.ReadLine()?.Trim();

            // 处理退出
            if (allowQuit && IsQuitCommand(input))
            {
                return quitValue;
            }

            // 验证数字输入
            if (int.TryParse(input, out int choice) && choice >= 1 && choice <= options.Count)
            {
                return options[choice - 1];
            }

            // 无效输入
            Console.WriteLine($"无效输入: \"{input}\"，请输入有效的序号。");
        }
    }

    /// <summary>
    /// 检查输入是否为退出命令。
    /// </summary>
    private static bool IsQuitCommand(string? input)
    {
        return input != null && 
               (input.Equals("q", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                input.Equals("exit", StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// 获取支持的退出关键词（用于提示）。
    /// </summary>
    private static string GetQuitWords()
    {
        return "'q', 'quit' 或 'exit'";
    }
}