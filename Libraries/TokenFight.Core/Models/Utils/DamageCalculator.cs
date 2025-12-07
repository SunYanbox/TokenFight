using TokenFight.Core.Constants;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models.Entities.Masters;

namespace TokenFight.Core.Models.Utils;

/// <summary>
/// 伤害计算构建器
/// </summary>
public sealed class DamageCalculator
{
    /// <summary> 是否应用技能倍率乘区 </summary>
    private bool _useRate = true;

    /// <summary> 是否计算防御减免乘区 </summary>
    private bool _useDefense = true;

    /// <summary> 是否应用增伤（来自攻击方）乘区 </summary>
    private bool _useDamageIncrease = true;

    /// <summary> 是否应用免伤（来自攻击方）乘区 </summary>
    private bool _useDamageReduction = true;

    /// <summary> 是否应用目标易伤乘区 </summary>
    private bool _useVulnerability = true;

    /// <summary> 是否应用穿透与抗性乘区（穿透 - 抗性） </summary>
    private bool _usePenetrationAndResistance = true;

    /// <summary> 是否进行暴击判定及暴伤计算 </summary>
    private bool _useCritical = true;

    /// <summary> 基础伤害是否为固定值 <br /> 设置后忽略`BaseAttrType`属性 <br /> 设置后以参数rate作为基础伤害 </summary>
    public bool FixBaseDamage = false;
    
    /// <summary> 基础属性类型（默认为攻击力） <br /> FixBaseDamage为false时使用该属性计算基础伤害 </summary>
    public AttrType BaseAttrType = AttrType.Attack;
    
    /// <summary> 伤害属性 </summary>
    public Element Element = Element.Physics;
    
    /// <summary> 伤害类型 </summary>
    public readonly EnumTypeMaster<DamageType> DamageTypes = new EnumTypeMaster<DamageType>([DamageType.NormalAttack]);
    
    /// <summary> 随机数生成器（用于暴击判定）</summary>
    public Random Random = new Random();

    /// <summary>
    /// 创建一个新的伤害计算构建器实例（每次调用应新建，以保证线程安全）
    /// </summary>
    public static DamageCalculator New => new DamageCalculator();

    /// <summary>
    /// 设置是否使用固定的基础伤害, 设置为true后忽略基础乘区
    /// </summary>
    public DamageCalculator WithFixBaseDamage(bool fixBaseDamage)
    {
        FixBaseDamage = fixBaseDamage;
        return this;
    }
    
    /// <summary>
    /// 设置是否启用倍率乘区
    /// </summary>
    /// <param name="enabled">为 true 时应用倍率，否则跳过</param>
    public DamageCalculator WithRate(bool enabled = true)
    {
        _useRate = enabled;
        return this;
    }

    /// <summary>
    /// 设置是否启用防御乘区
    /// </summary>
    /// <param name="enabled">为 true 时计算防御减免</param>
    public DamageCalculator WithDefense(bool enabled = true)
    {
        _useDefense = enabled;
        return this;
    }

    /// <summary>
    /// 设置是否启用增伤乘区（来自攻击方）
    /// </summary>
    /// <param name="enabled">为 true 时应用 DamageIncrease 属性</param>
    public DamageCalculator WithDamageIncrease(bool enabled = true)
    {
        _useDamageIncrease = enabled;
        return this;
    }

    /// <summary>
    /// 设置是否启用免伤乘区（注意：免伤通常属于防御方，此处逻辑按原代码保留为 source 免伤）
    /// </summary>
    /// <param name="enabled">为 true 时应用 DamageReduction 属性</param>
    public DamageCalculator WithDamageImmunity(bool enabled = true)
    {
        _useDamageReduction = enabled;
        return this;
    }

    /// <summary>
    /// 设置是否启用目标易伤乘区
    /// </summary>
    /// <param name="enabled">为 true 时应用目标的 Vulnerability 易伤属性</param>
    public DamageCalculator WithVulnerability(bool enabled = true)
    {
        _useVulnerability = enabled;
        return this;
    }

    /// <summary>
    /// 设置是否启用穿透与抗性乘区（计算 max(1 + 穿透 - 抗性, 0.001)）
    /// </summary>
    /// <param name="enabled">为 true 时应用穿透和抗性</param>
    public DamageCalculator WithPenetrationAndResistance(bool enabled = true)
    {
        _usePenetrationAndResistance = enabled;
        return this;
    }

    /// <summary>
    /// 设置伤害类型
    /// </summary>
    public DamageCalculator WithDamageTypes(params DamageType[] types)
    {
        DamageTypes.Clear();
        DamageTypes.AddRange(types);
        return this;
    }
    
    /// <summary>
    /// 设置伤害的属性类型
    /// </summary>
    public DamageCalculator WithElement(Element element)
    {
        Element = element;
        return this;
    }
    
    /// <summary>
    /// 设置是否启用暴击判定
    /// </summary>
    /// <param name="enabled">为 true 时根据暴击率判定是否暴击，并应用暴伤</param>
    public DamageCalculator WithCritical(bool enabled = true)
    {
        _useCritical = enabled;
        return this;
    }

    /// <summary>
    /// 设置基础属性类型（如 Attack、MaxHP 等）
    /// </summary>
    public DamageCalculator WithBaseAttr(AttrType type)
    {
        BaseAttrType = type;
        return this;
    }

    /// <summary>
    /// 设置随机数生成器（可用于确定性测试或避免线程竞争）
    /// </summary>
    public DamageCalculator WithRandom(Random random)
    {
        Random = random ?? throw new ArgumentNullException(nameof(random));
        return this;
    }

    /// <summary>
    /// 计算最终伤害值（调用前应已触发 DamageBefore 事件）
    /// </summary>
    public double Calculate(IActor source, IActor target, double rate, out bool isCrit)
    {
        if (!DamageHelper.CanActorCalculateAttribute(source) ||
            !DamageHelper.CanActorCalculateAttribute(target))
        {
            isCrit = false;
            return 0.0;
        }

        isCrit = false;
        double damage = FixBaseDamage ? rate : (source.AttrSet.GetAttr(BaseAttrType) * (_useRate ? rate : 1));
        
        if (_useCritical && Random.NextDouble() < source.AttrSet.GetAttr(AttrType.CriticalRate))
        {
            damage *= 1 + source.AttrSet.GetAttr(AttrType.CriticalDamage);
            isCrit = true;
        }
        else
        {
            DamageTypes.Remove(DamageType.CriticalDamage);
        }

        if (_useDefense)
        {
            double Zone(double lv) => lv * GameConst.DefenseK + GameConst.DefenseB;
            double effectiveDefense = Math.Max(0,
                target.AttrSet.GetAttr(AttrType.Defense) *
                (1 - target.AttrSet.GetAttr(AttrType.DefenseReduce)));
            damage *= Zone(source.Level) / (effectiveDefense + Zone(source.Level));
        }

        if (_useDamageIncrease)
        {
            double inc = source.AttrSet.GetAttr(AttrType.DamageIncrease); // 通用
            inc += source.AttrSet.GetAttr(Element, DamageModifierType.DamageIncrease); // 属性
            inc += DamageTypes.Select(x => source.AttrSet.GetAttr(x, DamageModifierType.DamageIncrease)).Sum();
            
            damage *= 1 + inc;
        }

        if (_useDamageReduction)
        {
            double red = target.AttrSet.GetAttr(AttrType.DamageReduction); // 通用
            red += target.AttrSet.GetAttr(Element, DamageModifierType.DamageReduction); // 属性
            red += DamageTypes.Select(x => target.AttrSet.GetAttr(x, DamageModifierType.DamageReduction)).Sum();
            
            damage *= Math.Max(1 - red, 0.001);
        }

        if (_useVulnerability)
        {
            double vul = source.AttrSet.GetAttr(AttrType.Vulnerability); // 通用
            vul += source.AttrSet.GetAttr(Element, DamageModifierType.Vulnerability);
            vul += DamageTypes.Select(x => source.AttrSet.GetAttr(x, DamageModifierType.Vulnerability)).Sum();
            
            damage *= 1 + vul;
        }

        if (_usePenetrationAndResistance)
        {
            double pen = source.AttrSet.GetAttr(AttrType.DamagePenetrate);
            pen += source.AttrSet.GetAttr(Element, DamageModifierType.Penetration);
            pen += DamageTypes.Select(x => source.AttrSet.GetAttr(x, DamageModifierType.Penetration)).Sum();
            
            double res = target.AttrSet.GetAttr(AttrType.DamageResistance);
            res += target.AttrSet.GetAttr(Element, DamageModifierType.Resistance);
            res += DamageTypes.Select(x => target.AttrSet.GetAttr(x, DamageModifierType.Resistance)).Sum();
            
            
            double netPen = pen - res;
            damage *= Math.Max(1 + netPen, 0.001);
        }

        source.AttrSet.ClearTempModify();
        target.AttrSet.ClearTempModify();

        return damage;
    }
}