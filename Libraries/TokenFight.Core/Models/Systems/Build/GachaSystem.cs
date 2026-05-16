using TokenFight.Core.Constants;
using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Databases.Models.Dungeons;
using TokenFight.Core.Databases.Models.Profiles;
using TokenFight.Core.Enums.Build;
using TokenFight.Core.Interfaces.Systems.Build;
using TokenFight.Core.Models.Build;

namespace TokenFight.Core.Models.Systems.Build;

public class GachaSystem(IDatabaseServer databaseServer) : IGachaSystem
{
    private readonly Random _random = new();
    public void Init() { }
    public void Reset() { }

    private GachaResult AddSaveAndRet(Profile profile, GachaResult result)
    {
        profile.Add(result);
        databaseServer.Save(profile);
        return result;
    }

    public GachaResult GachaOnce(Profile profile, GachaReward reward)
    {
        if (profile.Token < GameConst.GachaTokenCost) throw new Exception("Token不足");
        if (!reward.Verify()) throw new Exception($"抽卡奖励信息不完整: {reward}");
        profile.Token -= GameConst.GachaTokenCost;
        // 抽五星
        if (_random.NextDouble() < CalculateGachaRate(5, profile.GachaHistory.Star5GuaranteeCount))
        {
            List<string> star5 = reward.GetReward(GachaRate.Star5);
            List<string> star5Up = reward.GetReward(GachaRate.Star5Plus);
            List<string> c = new((profile.GachaHistory.IsStayTrue ? star5.Count : 0) + star5Up.Count);
            if (!profile.GachaHistory.IsStayTrue) c.AddRange(star5);
            c.AddRange(star5Up);
            string item = c[_random.Next(c.Count)];

            GachaRate rate = star5Up.Contains(item) ? GachaRate.Star5Plus : GachaRate.Star5;

            if (rate != GachaRate.Star5 || !(_random.NextDouble() < GameConst.GachaStar5SmallGuaranteeRate))
            {
                return AddSaveAndRet(profile, new GachaResult
                {
                    Rate = rate,
                    ItemTpl = item,
                    Count = 1,
                    Timestamp = DateTime.Now.Ticks
                });
            }
            item = star5Up[_random.Next(star5Up.Count)];
            rate = GachaRate.Star5Plus;

            return AddSaveAndRet(profile, new GachaResult
            {
                Rate = rate,
                ItemTpl = item,
                Count = 1,
                Timestamp = DateTime.Now.Ticks
            });
        }
        // 抽四星
        if (_random.NextDouble() < CalculateGachaRate(4, profile.GachaHistory.Star4GuaranteeCount))
        {
            List<string> star4 = reward.GetReward(GachaRate.Star4);

            return AddSaveAndRet(profile, new GachaResult
            {
                Rate = GachaRate.Star4,
                ItemTpl = star4[_random.Next(star4.Count)],
                Count = 1,
                Timestamp = DateTime.Now.Ticks
            });
        }

        List<string> star3 = reward.GetReward(GachaRate.Star3);
        return AddSaveAndRet(profile, new GachaResult
        {
            Rate = GachaRate.Star3,
            ItemTpl = star3[_random.Next(star3.Count)],
            Count = 1,
            Timestamp = DateTime.Now.Ticks
        });
    }

    public IEnumerable<GachaResult> Gacha(Profile profile, GachaReward reward, int count)
    {
        return Enumerable.Range(0, count).Select(_ => GachaOnce(profile, reward));
    }

    // rate = 4 | 5   hadGachaCount 为已抽卡次数
    public double CalculateGachaRate(int rate, int hadGachaCount)
    {
        hadGachaCount++; // 当前抽卡次数
        if (rate == 4)
        {
            return hadGachaCount < GameConst.GachaStar4Guarantee.Min
                ? GameConst.GachaStar4Rate
                : CalculateGachaRate(GameConst.GachaStar4Rate, hadGachaCount, GameConst.GachaStar4Guarantee);
        }
        if (rate == 5)
        {
            return hadGachaCount < GameConst.GachaStar5Guarantee.Min
                ? GameConst.GachaStar5Rate
                : CalculateGachaRate(GameConst.GachaStar5Rate, hadGachaCount, GameConst.GachaStar5Guarantee);
        }
        return 0;
    }

    /// <summary>
    /// 计算概率
    /// </summary>
    /// <param name="baseRate">基础概率</param>
    /// <param name="hadGachaCount">上一次出货后的总抽卡次数</param>
    /// <param name="guarantee">保底触发范围</param>
    /// <returns></returns>
    private double CalculateGachaRate(double baseRate, int hadGachaCount, TokenRange guarantee)
        => baseRate + (double)(hadGachaCount - guarantee.Min)
            / Math.Max(guarantee.Max - guarantee.Min, 1)
            * (1 - baseRate);
}
