using TokenFight.Core.Enums.Actions;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Helpers;

/// <summary>
/// 创建额外行动/追加攻击/终结技的辅助工具
/// </summary>
[AutoSysRegistryInit]
public static class CreateActionHelper
{
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }
    
    /// <summary>
    /// 获得一个普通回合
    /// </summary>
    public static void CreateNormal(IActor actor)
    {
        if (GameSystemRegistry == null) return;
        if (!ActorHelper.IsValidActor(actor)) return;
        
        GameSystemRegistry.ActionManagerSystem.CreateAction(ActionPriority.NormalOperations, actor: actor);
    }
    
    /// <summary>
    /// 获得一个额外回合
    /// </summary>
    public static void CreateNewExtraTurn(IActor actor)
    {
        if (GameSystemRegistry == null) return;
        if (!ActorHelper.IsValidActor(actor)) return;
        
        GameSystemRegistry.ActionManagerSystem.CreateAction(ActionPriority.ExtraTurn, actor: actor);
    }

    /// <summary>
    /// 释放一次追加攻击
    /// </summary>
    public static void CreateFollowUpAttack(ISkill skill)
    {
        if (GameSystemRegistry == null) return;
        if (!ActorHelper.IsValidActor(skill.Source)) return;
        
        GameSystemRegistry.ActionManagerSystem.CreateAction(ActionPriority.FollowUpAttack, skill: skill);
    }
    
    /// <summary>
    /// 释放一次终结技
    /// </summary>
    public static void CreateUltimate(ISkill skill)
    {
        if (GameSystemRegistry == null) return;
        if (!ActorHelper.IsValidActor(skill.Source)) return;
        
        GameSystemRegistry.ActionManagerSystem.CreateAction(ActionPriority.Ultimate, skill: skill);
    }
}