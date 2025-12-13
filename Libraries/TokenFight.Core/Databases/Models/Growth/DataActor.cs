using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Effects;

namespace TokenFight.Core.Databases.Models.Growth;

public class DataActor: IDataActor
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required Dictionary<AttrType, GrowthBase> AttrGrowth { get; set; }
    public required Dictionary<SkillType, Dictionary<string, SkillEffectData>> SkillGrowth { get; set; }

    public Dictionary<string, string>? DataString { get; set; }
    public Dictionary<string, double>? DataDouble { get; set; }
    public Dictionary<string, int>? DataInt { get; set; }
    public Dictionary<string, bool>? DataBool { get; set; }
    public Dictionary<string, GrowthBase>? Growths { get; set; }


    public double GetAttrGrowth(AttrType attrType, int level) => AttrGrowth.TryGetValue(attrType, out GrowthBase? growthBase) ? growthBase.Calculate(level) : 0;

    public Dictionary<string, SkillEffectData> GetSkillEffectData(SkillType skillType) => SkillGrowth.GetValueOrDefault(skillType) ?? [];
}