using System.Text.Json.Serialization;
using TokenFight.Core.Enums.Build;
using TokenFight.Core.Models.Build;

namespace TokenFight.Core.Databases.Models.Profiles;

/// <summary> 抽卡历史记录 </summary>
public sealed class GachaHistory
{
    public GachaHistory()
    {
        History = [];
        Star4GuaranteeCount = Star5GuaranteeCount = 0;
        IsStayTrue = false;
    }

    [JsonConstructor]
    public GachaHistory(List<GachaResult> history, int star4GuaranteeCount, int star5GuaranteeCount, bool isStayTrue)
    {
        History = new List<GachaResult>(history);
        Star4GuaranteeCount = star4GuaranteeCount;
        Star5GuaranteeCount = star5GuaranteeCount;
        IsStayTrue = isStayTrue;
    }
    /// <summary> 历史抽卡记录 </summary>
    public List<GachaResult> History { get; init; }
    /// <summary> 四星未抽出来的次数 </summary>
    public int Star4GuaranteeCount { get; private set; }
    /// <summary> 五星未抽出来的次数 </summary>
    public int Star5GuaranteeCount { get; private set; }
    /// <summary> 是否保底出Up五星 </summary>
    public bool IsStayTrue { get; private set; }

    [JsonIgnore]
    public int Count => History.Count;

    public void Add(GachaResult result)
    {
        History.Add(result);
        switch (result.Rate)
        {
            case GachaRate.Star3:
                Star4GuaranteeCount++;
                Star5GuaranteeCount++;
                break;
            case GachaRate.Star4:
                Star4GuaranteeCount = 0;
                Star5GuaranteeCount++;
                break;
            case GachaRate.Star5:
                Star4GuaranteeCount++;
                Star5GuaranteeCount = 0;
                IsStayTrue = true;
                break;
            case GachaRate.Star5Plus:
                Star4GuaranteeCount++;
                Star5GuaranteeCount = 0;
                IsStayTrue = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result.Rate), result.Rate, null);
        }
    }
}