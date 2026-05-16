using TokenFight.Core.Enums.Build;

namespace TokenFight.Core.Models.Build;

/// <summary> 抽卡奖励信息 </summary>
public class GachaReward
{
    /// <summary> 卡池ID </summary>
    public required string Id { get; set; }
    /// <summary> 卡池名称 </summary>
    public required string Name { get; set; }
    public required List<string> Star5Plus { get; set; }
    public required List<string> Star5 { get; set; }
    public required List<string> Star4 { get; set; }
    public required List<string> Star3 { get; set; }

    /// <summary> 验证抽卡奖励信息 | 每个星级的奖励数量不能为0 </summary>
    public bool Verify() => Star5Plus.Count > 0
                            && Star5.Count > 0
                            && Star4.Count > 0
                            && Star3.Count > 0;

    public List<string> GetReward(GachaRate rate)
    {
        return rate switch
        {
            GachaRate.Star5Plus => Star5Plus,
            GachaRate.Star5 => Star5,
            GachaRate.Star4 => Star4,
            GachaRate.Star3 => Star3,
            _ => throw new ArgumentOutOfRangeException(nameof(rate), rate, null)
        };
    }
}
