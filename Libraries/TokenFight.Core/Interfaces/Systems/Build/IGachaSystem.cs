using TokenFight.Core.Databases.Models.Profiles;
using TokenFight.Core.Interfaces.Bases;
using TokenFight.Core.Models.Build;

namespace TokenFight.Core.Interfaces.Systems.Build;

public interface IGachaSystem: ISystem
{
    /// <summary> 抽奖一次, 并在记录历史记录后返回抽奖结果 </summary>
    public GachaResult GachaOnce(Profile profile, GachaReward reward);

    /// <summary> 抽奖count次, 并记录历史记录 </summary>
    public IEnumerable<GachaResult> Gacha(Profile profile, GachaReward reward, int count);
    
    /// <summary> 计算概率 </summary>
    public double CalculateGachaRate(int rate, int hadGachaCount);
}