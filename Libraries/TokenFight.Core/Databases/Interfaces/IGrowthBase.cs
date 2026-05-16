using System.Text.Json.Serialization;
using TokenFight.Core.Databases.Models.Growth;

namespace TokenFight.Core.Databases.Interfaces;

/// <summary>
/// 角色/技能/属性的成长模型接口
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(GrowthBase), "GrowthBase")]
public interface IGrowthBase
{
    [JsonPropertyName("MinLevel")]
    int MinLevel { get; set; }

    [JsonPropertyName("MaxLevel")]
    int MaxLevel { get; set; }

    // 线性
    [JsonPropertyName("BaseValue")]
    double? BaseValue { get; set; }

    [JsonPropertyName("GrowthPerLevel")]
    double? GrowthPerLevel { get; set; }

    // 指数
    [JsonPropertyName("ExpBase")]
    double? ExpBase { get; set; }

    [JsonPropertyName("GrowthFactor")]
    double? GrowthFactor { get; set; }

    // 对数
    [JsonPropertyName("LogBaseValue")]
    double? LogBaseValue { get; set; }

    [JsonPropertyName("ScaleFactor")]
    double? ScaleFactor { get; set; }

    [JsonPropertyName("LogBase")]
    double? LogBase { get; set; }

    // 幂函数
    [JsonPropertyName("Offset")]
    double? Offset { get; set; }

    [JsonPropertyName("Multiplier")]
    double? Multiplier { get; set; }

    [JsonPropertyName("Power")]
    double? Power { get; set; }

    // 分段线性
    [JsonPropertyName("Segments")]
    List<Segment>? Segments { get; set; }

    /// <summary>
    /// 分段线性养成数据
    /// </summary>
    public class Segment
    {
        [JsonPropertyName("MaxLevel")]
        public int MaxLevel { get; set; }

        [JsonPropertyName("ValueDelta")]
        public double ValueDelta { get; set; }

        [JsonPropertyName("GrowthPerLevel")]
        public double GrowthPerLevel { get; set; }
    }

    /// <summary>返回当前配置的成长类型名称</summary>
    string GrowthType { get; }

    /// <summary>根据等级计算成长值</summary>
    double Calculate(int level);

    /// <summary>获取成长参数的文本描述</summary>
    string GetGrowthDescription();

    /// <summary>生成成长曲线预览</summary>
    string GetGrowthPreview(int sampleCount = 5);
}
