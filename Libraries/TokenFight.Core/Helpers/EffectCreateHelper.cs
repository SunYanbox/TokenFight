using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Effects.Effects;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Helpers;

/// <summary> 创建效果的辅助工具 </summary>
[AutoSysRegistryInit]
public static class EffectCreateHelper
{
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }

    /// <summary>
    /// 创建一个基于百分比增益的基础效果实例
    /// </summary>
    /// <param name="id">效果的唯一标识符</param>
    /// <param name="source">效果的施加者</param>
    /// <param name="target">效果的目标对象</param>
    /// <param name="modifyData">修改数据字典，键为属性类型，值为修改百分比</param>
    /// <param name="duration">效果持续回合数，-1表示不启用持续回合数</param>
    /// <param name="initStack">初始堆叠层数，-1表示不启用堆叠</param>
    /// <param name="maxStack">最大堆叠层数，-1表示不启用堆叠</param>
    /// <param name="deltaStack">每次结算时堆叠变化量</param>
    /// <param name="mark">效果标记，-1表示无标记</param>
    /// <returns>创建的BaseEffectPctGain效果实例</returns>
    public static BaseEffectPctGain CreatePctGain(
        string id, IActor source, IActor target, Dictionary<int, double> modifyData,
        int duration = -1, int initStack = -1, int maxStack = -1, int deltaStack = -1, int mark = -1)
    {
        var effect = new BaseEffectPctGain(source, target, id, modifyData, GameSystemRegistry!);

        if (duration >= 1)
        {
            effect.InitDuration(duration);
        }

        if (initStack >= 1 && maxStack >= 1)
        {
            effect.InitStack(initStack, maxStack, deltaStack);
        }

        if (mark >= 1)
        {
            effect.InitMark(mark);
        }
        return effect;
    }

    /// <summary>
    /// 创建一个基于标记效果实例
    /// </summary>
    /// <param name="id">效果的唯一标识符</param>
    /// <param name="source">效果的施加者</param>
    /// <param name="target">效果的目标对象</param>
    /// <param name="mark">效果标记，-1表示无标记</param>
    /// <param name="duration">效果持续回合数，-1表示不启用持续回合数</param>
    /// <param name="initStack">初始堆叠层数，-1表示不启用堆叠</param>
    /// <param name="maxStack">最大堆叠层数，-1表示不启用堆叠</param>
    /// <param name="deltaStack">每次结算时堆叠变化量</param>
    /// <returns>创建的BaseEffectMark效果实例</returns>
    public static BaseEffectMark CreateMark(string id, IActor source, IActor target,
        int mark = 1, int duration = -1, int initStack = -1, int maxStack = -1, int deltaStack = -1)
    {
        var effect = new BaseEffectMark(source, target, id, GameSystemRegistry!);

        if (mark >= 1)
        {
            effect.LifeCycle!.InitMark(mark);
            effect.Type = EffectType.Mark;
        }

        if (duration >= 1)
        {
            effect.LifeCycle!.InitDuration(duration);
        }

        if (initStack >= 1 && maxStack >= 1)
        {
            effect.LifeCycle!.InitStack(initStack, maxStack, deltaStack);
        }


        return effect;
    }
}
