using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Helpers;

/// <summary>
/// 释放效果的工具
/// </summary>
[AutoSysRegistryInit]
public static class EffectHelper
{
    public static readonly Random Random = new();
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }

    /// <summary> 向指定目标释放效果 </summary>
    public static void TakeEffectSingle(IEffect effect)
    {
        if (GameSystemRegistry == null) return;
        if (!ActorHelper.IsValidActor(ActorHelper.GetActorFromWeakRef(effect.Source))
            || !ActorHelper.IsValidActor(
                ActorHelper.GetActorFromWeakRef(effect.Target))) return;

        ActorHelper.GetActorFromWeakRef(effect.Target)?.EffectMaster.Apply(effect);
    }

    /// <summary> 移除目标身上指定类型的count个效果 | 如果count==-1, 移除所有对应类型效果 </summary>
    public static void RemoveEffects(IActor target, EffectType type, int count = -1)
    {
        HashSet<string> effects = target.EffectMaster.GetEffectIds(type);
        if (count == -1)
        {
            target.EffectMaster.Remove(effects);
        }
        else if (count > 0)
        {
            int removeCount = Math.Min(effects.Count, count);
            HashSet<string> effectsToRemove = [];
            List<string> effectList = effects.ToList();
            for (int i = 0; i < removeCount; i++)
            {
                int index = Random.Next(effectList.Count);
                effectsToRemove.Add(effectList[index]);
                effectList.RemoveAt(index);
            }
            target.EffectMaster.Remove(effectsToRemove);
        }
    }
}