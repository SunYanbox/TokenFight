using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Models.Attrs;

namespace TokenFight.Core.Interfaces.Attrs;

/// <summary> 属性接口 </summary>
public interface IAttrSet
{
    /// <summary> 获取基础属性 </summary>
    double GetBaseAttr<T>(T type, DamageModifierType? modifierType = null) where T: struct, Enum;

    /// <summary> 获取增益属性 </summary>
    double GetGainAttr<T>(T type, DamageModifierType? modifierType = null) where T: struct, Enum;

    /// <summary> 获取最终属性(包含临时增益) </summary>
    double GetAttr<T>(T type, DamageModifierType? modifierType = null) where T: struct, Enum;

    /// <summary> 覆盖式设置属性 </summary>
    public void SetAttr(AttrModifyData data);
    
    /// <summary> 清理所有临时增益 </summary>
    public void ClearTempModify();

    /// <summary> 移除指定来源的所有增益 </summary>
    public void Remove(string id);

    /// <summary> 将属性枚举转换为整型 </summary>
    /// <typeparam name="T">属性枚举 限制: AttrType, DamageType, Element</typeparam>
    /// <param name="type">属性枚举</param>
    /// <param name="modifierType">伤害修饰类型；当 T 为 DamageType 或 Element 时必须提供</param>
    public static int ToInt<T>(T type, DamageModifierType? modifierType = null) where T : struct, Enum
    {
        if (type is AttrType attrType)
        {
            return (int)attrType;
        }

        if (type is DamageType damageType && modifierType.HasValue)
        {
            return (int)damageType + (int)modifierType.Value;
        }

        if (type is Element element && modifierType.HasValue)
        {
            return (int)element + (int)modifierType.Value;
        }

        throw new ArgumentException($"无法将枚举 {typeof(T)} 转换为整型，或缺少必要的 modifierType 参数: ", nameof(type));
    }
}