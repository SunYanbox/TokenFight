using System.Text;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Interfaces.Attrs;

namespace TokenFight.Core.Models.Attrs;

/// <summary>
/// 属性系统
/// 管理游戏实体的属性系统，支持三种修改类型：
/// Base（基础值）
/// Flat（固定值加成）
/// Percent（百分比加成）
/// 支持临时和永久属性修改
/// </summary>
public class AttrSet: IAttrSet
{
    private static readonly HashSet<int> AttrIndexes = Enum.GetValues<AttrType>().Select(x => IAttrSet.ToInt(x)).ToHashSet();
    private static readonly HashSet<int> DamageIndexes =
        Enum.GetValues<DamageType>()
            .SelectMany(damageType =>
                Enum.GetValues<DamageModifierType>()
                    .Select(modifierType => IAttrSet.ToInt(damageType, modifierType)))
            .ToHashSet();
    private static readonly HashSet<int> ElementIndexes =
        Enum.GetValues<Element>()
            .SelectMany(element =>
                Enum.GetValues<DamageModifierType>()
                    .Select(modifierType => IAttrSet.ToInt(element, modifierType)))
            .ToHashSet();
    protected static readonly HashSet<int> AttrAllIndexes = AttrIndexes.Concat(DamageIndexes).Concat(ElementIndexes).ToHashSet();

    // 基础属性字典 [来源ID -> [属性类型 -> 加成值]]
    private readonly DictManager _baseAttribute;

    // 固定值加成字典 [来源ID -> [属性类型 -> 加成值]]
    private readonly DictManager _flatAttribute;

    // 百分比加成字典 [来源ID -> [属性类型 -> 加成百分比]]
    private readonly DictManager _percentAttribute;

    // 临时加成字典
    private readonly DictManager _baseModifiers;
    private readonly DictManager _flatModifiers;
    private readonly DictManager _percentModifiers;

    /// <summary> 构造函数 </summary>
    public AttrSet()
    {
        // 属性
        _baseAttribute = new DictManager();
        _flatAttribute = new DictManager();
        _percentAttribute = new DictManager();
        _baseAttribute.Init(AttrAllIndexes);
        _flatAttribute.Init(AttrAllIndexes);
        _percentAttribute.Init(AttrAllIndexes);
        // 临时属性
        _baseModifiers = new DictManager();
        _flatModifiers = new DictManager();
        _percentModifiers = new DictManager();
        _baseModifiers.Init(AttrAllIndexes);
        _flatModifiers.Init(AttrAllIndexes);
        _percentModifiers.Init(AttrAllIndexes);
    }

    private double TryRetOrZero(Func<double> func)
    {
        try
        {
            return func();
        }
        catch (Exception e)
        {
            return 0;
        }
    }

    private double BaseValue(int type)
    {
        return TryRetOrZero(() => _baseAttribute.GetValues(type).Sum() + _baseModifiers.GetValues(type).Sum());
    }
    private double FlatValue(int type)
    {
        return TryRetOrZero(() => _flatAttribute.GetValues(type).Sum() + _flatModifiers.GetValues(type).Sum());
    }
    private double PercentValue(int type)
    {
        return TryRetOrZero(() => _percentAttribute.GetValues(type).Sum() + _percentModifiers.GetValues(type).Sum());
    }

    /// <summary> 清理所有临时增益 </summary>
    public void ClearTempModify()
    {
        _baseModifiers.ClearValues();
        _flatModifiers.ClearValues();
        _percentModifiers.ClearValues();
    }
    public double GetBaseAttr<T>(T type, DamageModifierType? modifierType = null) where T : struct, Enum
    {
        try
        {
            return BaseValue(IAttrSet.ToInt(type, modifierType));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    public double GetGainAttr<T>(T type, DamageModifierType? modifierType = null) where T : struct, Enum
    {
        try
        {
            int attr = IAttrSet.ToInt(type, modifierType);
            return BaseValue(attr)
                   * PercentValue(attr)
                   + FlatValue(attr);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    public double GetAttr<T>(T type, DamageModifierType? modifierType = null) where T : struct, Enum
    {
        try
        {
            int attr = IAttrSet.ToInt(type, modifierType);
            return BaseValue(attr)
                   * (1 + PercentValue(attr))
                   + FlatValue(attr);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return 0;
        }
    }

    /// <summary> 覆盖式设置属性 </summary>
    public void SetAttr(AttrModifyData data)
    {
        if (string.IsNullOrEmpty(data.Id)) return;
        foreach ((int type, double value) in data.ModifyData)
        {
            switch (data.Type)
            {
                case AttrModifyType.Base:
                    (data.IsTemp ? _baseModifiers : _baseAttribute).Add(type, data.Id, value);
                    break;
                case AttrModifyType.Flat:
                    (data.IsTemp ? _flatModifiers : _flatAttribute).Add(type, data.Id, value);
                    break;
                case AttrModifyType.Percent:
                    (data.IsTemp ? _percentModifiers : _percentAttribute).Add(type, data.Id, value);
                    break;
            }
        }
    }

    /// <summary> 移除指定来源的所有增益 </summary>
    public void Remove(string id)
    {
        foreach (DictManager dictManager in new List<DictManager>
                 {
                     _baseAttribute,
                     _baseModifiers,
                     _flatAttribute,
                     _flatModifiers,
                     _percentAttribute,
                     _percentModifiers
                 })
        {
            dictManager.RemoveByKey(id);
        }
    }

    /// <summary> 字符串化 </summary>
    public override string ToString() => ToDetailString();

    public string ToDetailString(HashSet<object>? visited = null)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append("AttrSet( ");
        if (_baseAttribute.Count > 0)
        {
            IEnumerable<string> lines = Enum.GetValues<AttrType>()
                .Select(type => $"{type}: {GetAttr(type):F2}");
            stringBuilder.AppendJoin(", ", lines);
            AppendModifierAttributes(stringBuilder, Enum.GetValues<DamageType>(),
                (dt, mod) => GetAttr(dt, mod),
                (dt, mod) => $"{dt}{mod}");
            AppendModifierAttributes(stringBuilder, Enum.GetValues<Element>(),
                (e, mod) => GetAttr(e, mod),
                (e, mod) => $"{e}{mod}");
        }
        stringBuilder.Append(" )");
        return stringBuilder.ToString();
    }

    private void AppendModifierAttributes<T>(
        StringBuilder sb,
        T[] types,
        Func<T, DamageModifierType, double> getValue,
        Func<T, DamageModifierType, string> formatKey) where T : struct, Enum
    {
        IEnumerable<string> lines = types.Select(type =>
            string.Join(", ",
                Enum.GetValues<DamageModifierType>()
                    .Select(mod =>
                    {
                        double value = getValue(type, mod);
                        return Math.Abs(value) < double.Epsilon ? null : $"{formatKey(type, mod)}: {value:P2}";
                    })
                // .Where(s => s != null) // 配合上面的 null 判断
            )
        );

        sb.AppendJoin(", ", lines);
    }

    public string ToShortString(HashSet<object>? visited = null)
    {
        string msg = "AttrSet { ";
        msg += $"Count: {_baseAttribute.Count}";
        return msg + " }";
    }
}