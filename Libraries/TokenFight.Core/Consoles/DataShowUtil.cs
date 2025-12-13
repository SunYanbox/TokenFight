using TokenFight.Core.Consoles.Details;
using TokenFight.Core.Constants;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;

namespace TokenFight.Core.Consoles;

/// <summary>
/// 展示信息的工具
/// </summary>
public static class DataShowUtil
{
    /// <summary> 展示行动值, 限制在0~GameConst.ActionValueMaxValue之间 </summary>
    public static string ShowActionValue(double actionValue) => $"{Math.Min(Math.Max(actionValue, 0), GameConst.ActionValueMaxValue):F2}";

    /// <summary> 展示战场详细信息 </summary>
    public static void ShowGameField(GameSystemRegistry gameSystemRegistry)
    {
        foreach (IActor actor in ActorHelper.SortActorsByPosition(gameSystemRegistry.ActorManagerSystem.AllPlayers.Values.ToArray()))
        {
            Console.Write("- ");
            actor.DisplayActorInfo();
            Console.WriteLine();
        }
        Console.WriteLine();
        foreach (IActor actor in ActorHelper.SortActorsByPosition(gameSystemRegistry.ActorManagerSystem.AllEnemies.Values.ToArray()))
        {
            Console.Write("- ");
            actor.DisplayActorInfo();
            Console.WriteLine();
        }

        Console.WriteLine();
        Console.WriteLine(ToDetailStringUtil.DumpsActionListSystem(gameSystemRegistry.ActionListSystem));

        Console.WriteLine(
            $"剩余玩家数量: {gameSystemRegistry.ActorPoolSystem.PlayerCount}, 敌人数量: {gameSystemRegistry.ActorManagerSystem.AllEnemies.Count + gameSystemRegistry.ActorPoolSystem.EnemyCount}/{gameSystemRegistry.ActorPoolSystem.EnemyMaxCount}, 波次: {gameSystemRegistry.ActorPoolSystem.Wave + 1}/{gameSystemRegistry.ActorPoolSystem.WaveMax}");
        // Console.WriteLine(ToDetailStringUtil.DumpsActionManagerSystem(gameSystemRegistry.ActionManagerSystem));
        ToDetailStringUtil.OutputActionManagerSystem(gameSystemRegistry.ActionManagerSystem);

        Console.Write($"战技点: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{gameSystemRegistry.GlobalResourcesSystem.SkillPoint}");
        Console.ResetColor();
        Console.Write($"/{gameSystemRegistry.GlobalResourcesSystem.MaxSkillPoint} ");

        Console.Write($"行动值: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($"{gameSystemRegistry.GlobalResourcesSystem.ActionValue}");
        Console.ResetColor();
        Console.WriteLine();
    }
}