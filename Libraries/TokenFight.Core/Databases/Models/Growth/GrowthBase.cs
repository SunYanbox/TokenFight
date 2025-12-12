using System.Text.Json.Serialization;
using TokenFight.Core.Databases.Interfaces;

namespace TokenFight.Core.Databases.Models.Growth;

/// <summary>角色/技能的成长方式</summary>
public class GrowthBase: IGrowthBase
{
    [JsonPropertyName("MinLevel")]
    public int MinLevel { get; set; }
    [JsonPropertyName("MaxLevel")]
    public int MaxLevel { get; set; }
    
    // 线性成长参数
    [JsonPropertyName("BaseValue")]
    public double? BaseValue { get; set; }
    [JsonPropertyName("GrowthPerLevel")]
    public double? GrowthPerLevel { get; set; }
    
    // 指数成长参数
    [JsonPropertyName("ExpBase")]
    public double? ExpBase { get; set; }
    [JsonPropertyName("GrowthFactor")]
    public double? GrowthFactor { get; set; }
    
    // 对数成长参数
    [JsonPropertyName("LogBaseValue")]
    public double? LogBaseValue { get; set; }
    [JsonPropertyName("ScaleFactor")]
    public double? ScaleFactor { get; set; }
    [JsonPropertyName("LogBase")]
    public double? LogBase { get; set; }
    
    // 混合成长参数（用于更复杂的曲线）
    [JsonPropertyName("Offset")]
    public double? Offset { get; set; }
    [JsonPropertyName("Multiplier")]
    public double? Multiplier { get; set; }
    [JsonPropertyName("Power")]
    public double? Power { get; set; }

    // 分段线性养成
    [JsonPropertyName("Segments")]
    public List<IGrowthBase.Segment>? Segments { get; set; }
    
    [JsonIgnore] private bool IsLinear => BaseValue != null && GrowthPerLevel != null;
    
    [JsonIgnore]
    private bool IsExponential => ExpBase != null && GrowthFactor != null;
    
    [JsonIgnore]
    private bool IsLogarithmic => LogBaseValue != null && ScaleFactor != null;
    
    [JsonIgnore]
    private bool IsPowerBased => Offset != null && Multiplier != null && Power != null;
    
    [JsonIgnore]
    private bool IsSegmentedLinear => Segments != null;
    
    [JsonIgnore]
    public string GrowthType
    {
        get
        {
            if (IsLinear) return "Linear";
            if (IsExponential) return "Exponential";
            if (IsLogarithmic) return "Logarithmic";
            if (IsPowerBased) return "PowerBased";
            if (IsSegmentedLinear) return "SegmentedLinear";
            return "Unknown";
        }
    }

    public double Calculate(int level)
    {
        level = Math.Clamp(level, MinLevel, MaxLevel);
        
        if (IsLinear)
        {
            return CalculateLinear(level);
        }
        else if (IsExponential)
        {
            return CalculateExponential(level);
        }
        else if (IsLogarithmic)
        {
            return CalculateLogarithmic(level);
        }
        else if (IsPowerBased)
        {
            return CalculatePowerBased(level);
        }
        else if (IsSegmentedLinear)
        {
            return CalculateSegmentedLinear(level);
        }
        else
        {
            throw new InvalidOperationException("未配置有效的成长参数");
        }
    }
    
    private double CalculateLinear(int level)
    {
        return BaseValue!.Value + (level - 1) * GrowthPerLevel!.Value;
    }

    private double CalculateSegmentedLinear(int level)
    {
        double value = 0;
        foreach (IGrowthBase.Segment segment in Segments!)
        {
            if (level > segment.MaxLevel) break;
            value += segment.ValueDelta + segment.GrowthPerLevel * (segment.MaxLevel - level);
        }
        return value;
    }
    
    private double CalculateExponential(int level)
    {
        return ExpBase!.Value * Math.Pow(GrowthFactor!.Value, level - 1);
    }
    
    private double CalculateLogarithmic(int level)
    {
        double logValue = LogBase != null ? 
            Math.Log(level, LogBase.Value) : 
            Math.Log10(level);
        return LogBaseValue!.Value + ScaleFactor!.Value * logValue;
    }
    
    private double CalculatePowerBased(int level)
    {
        return Offset!.Value + Multiplier!.Value * Math.Pow(level, Power!.Value);
    }

    public string GetGrowthDescription()
    {
        return GrowthType switch
        {
            "Linear" => $"线性成长: 基础值={BaseValue}, 每级成长={GrowthPerLevel}, 等级范围={MinLevel}-{MaxLevel}",
            "Exponential" => $"指数成长: 基础值={ExpBase}, 成长系数={GrowthFactor}, 等级范围={MinLevel}-{MaxLevel}",
            "Logarithmic" => $"对数成长: 基础值={LogBaseValue}, 缩放系数={ScaleFactor}, 对数底数={LogBase ?? 10}, 等级范围={MinLevel}-{MaxLevel}",
            "PowerBased" => $"幂次成长: 偏移={Offset}, 倍数={Multiplier}, 幂次={Power}, 等级范围={MinLevel}-{MaxLevel}",
            _ => "未知成长类型或参数配置不完整"
        };
    }
    
    /// <summary>获取成长曲线的预览数据</summary>
    public string GetGrowthPreview(int sampleCount = 5)
    {
        if (sampleCount < 2) sampleCount = 2;
        if (sampleCount > MaxLevel - MinLevel + 1) sampleCount = MaxLevel - MinLevel + 1;
        
        var step = (MaxLevel - MinLevel) / (sampleCount - 1);
        var preview = new System.Text.StringBuilder();
        preview.AppendLine($"{GetGrowthDescription()} 预览:");
        
        for (int i = 0; i < sampleCount; i++)
        {
            var level = MinLevel + i * step;
            if (level > MaxLevel) level = MaxLevel;
            var value = Calculate(level);
            preview.AppendLine($"  等级 {level}: {value:F2}");
        }
        
        return preview.ToString();
    }
}