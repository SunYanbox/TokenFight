using TokenFight.Core.Enums.Build;

namespace TokenFight.Core.Models.Build;

/// <summary> 抽卡结果 </summary>
public class GachaResult
{
    /// <summary> 抽卡的品质 </summary>
    public GachaRate Rate { get; set; }
    /// <summary> 获得的物品 <br /> 为null表示没有抽到资源 </summary>
    public string? ItemTpl { get; set; }
    /// <summary> 抽卡获得的资源数量 </summary>
    public int Count { get; set; }
    /// <summary> 抽卡的时间戳 </summary>
    public long Timestamp { get; set; }
}