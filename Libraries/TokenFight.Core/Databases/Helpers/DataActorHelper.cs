using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Databases.Models;
using TokenFight.Core.Databases.Models.Growth;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Attrs;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models.Attrs;

namespace TokenFight.Core.Databases.Helpers;

/// <summary> 角色数据访问助手类 </summary>
public static class DataActorHelper
{
    /// <summary> 通过DataActor数据初始化Actor的养成属性 </summary>
    public static void InitAttrSet(IActor actor, DataActor dataActor)
    {
        Dictionary<int, double> modify = new Dictionary<int, double>();
        foreach ((AttrType type, GrowthBase value) in dataActor.AttrGrowth)
        {
            double v = value.Calculate(actor.Level);
            if (type == AttrType.Health)
                v = Math.Max(v, 1);
            modify.TryAdd(IAttrSet.ToInt(type), v);
        }
        actor.AttrSet.SetAttr(new AttrModifyData
        {
            IsTemp = false,
            ModifyData = modify,
            Type = AttrModifyType.Base,
            Id = "base"
        });
    }
    
    /// <summary>
    /// 安全地获取技能效果数据
    /// </summary>
    /// <param name="dataActor">角色数据</param>
    /// <param name="skillType">技能类型</param>
    /// <param name="skillId">技能ID</param>
    /// <returns>技能效果数据，如果不存在则返回null</returns>
    public static SkillEffectData? GetSkillEffectData(
        DataActor dataActor, 
        SkillType skillType, 
        string skillId)
    {
        if (!dataActor.SkillGrowth.TryGetValue(skillType, out var skillDict)) return null;
        skillDict.TryGetValue(skillId, out SkillEffectData? skillData);
        return skillData;
    }

    /// <summary>
    /// 格式化技能描述
    /// </summary>
    /// <param name="skillEffectData">技能效果数据</param>
    /// <param name="args">参数</param>
    /// <returns>格式化后的技能描述</returns>
    public static string FormatSkillDesc(SkillEffectData skillEffectData, object?[] args)
    {
        return string.Format(skillEffectData.Desc, args);
    }
    
    /// <summary>
    /// 获取技能成长值
    /// </summary>
    /// <param name="skillData">技能数据</param>
    /// <param name="growthKey">成长键</param>
    /// <param name="level">等级</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>计算后的成长值</returns>
    public static double GetGrowthValue(
        SkillEffectData skillData, 
        string growthKey, 
        int level, 
        double defaultValue = 0.0)
    {
        if (skillData.Growths.TryGetValue(growthKey, out var growth))
        {
            return growth.Calculate(level);
        }
        return defaultValue;
    }

    /// <summary>
    /// 获取指定类型的常量数据
    /// </summary>
    /// <param name="dataActor">角色数据</param>
    /// <param name="key">数据键</param>
    /// <typeparam name="T">double / int / bool / string</typeparam>
    /// <returns></returns>
    public static T? GetExtendProperty<T>(DataActor dataActor, string key)
    {
        if (typeof(T) == typeof(double))
            return (T?)(object?)dataActor.DataDouble?.GetValueOrDefault(key, 0.0);

        if (typeof(T) == typeof(int))
            return (T?)(object?)dataActor.DataInt?.GetValueOrDefault(key, 0);
        
        if (typeof(T) == typeof(bool)) 
            return (T?)(object)dataActor.DataBool?.GetValueOrDefault(key, false);
        
        if (typeof(T) == typeof(string))
            return (T?)(object?)dataActor.DataString?.GetValueOrDefault(key, "");

        return (T?)(object?)null;
    }
}
