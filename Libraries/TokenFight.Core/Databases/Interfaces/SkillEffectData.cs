using TokenFight.Core.Databases.Models;
using TokenFight.Core.Databases.Models.Growth;

namespace TokenFight.Core.Databases.Interfaces;

public class SkillEffectData
{
    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required double Charge { get; set; }
    public required int SkillPointDelta { get; set; }
    public required Dictionary<string, GrowthBase> Growths { get; set; }
}