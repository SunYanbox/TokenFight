using System.Text.Json.Serialization;
using TokenFight.Core.Databases.Models.Growth;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Effects;

namespace TokenFight.Core.Databases.Interfaces;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(DataActor), "DataActor")]
public interface IDataActor
{
    [JsonPropertyName("Id")]
    string Id { get; set; }
    [JsonPropertyName("Name")]
    string Name { get; set; }
    [JsonPropertyName("Desc")]
    string Desc { get; set; }
    /// <summary> 角色属性养成数据 </summary>
    public Dictionary<AttrType, GrowthBase> AttrGrowth { get; set; }
    /// <summary> 技能养成数据 </summary>
    public Dictionary<SkillType, Dictionary<string, SkillEffectData>> SkillGrowth { get; set; }
    /// <summary> 常量数据 </summary>
    [JsonPropertyName("DataString")]
    Dictionary<string, string>? DataString { get; set; }
    /// <summary> 常量数据 </summary>
    [JsonPropertyName("DataDouble")]
    Dictionary<string, double>? DataDouble { get; set; }
    /// <summary> 常量数据 </summary>
    [JsonPropertyName("DataInt")]
    Dictionary<string, int>? DataInt { get; set; }
    /// <summary> 常量数据 </summary>
    [JsonPropertyName("DataBool")]
    Dictionary<string, bool>? DataBool { get; set; }
    /// <summary> 成长数据 </summary>
    [JsonPropertyName("Growths")]
    Dictionary<string, GrowthBase>? Growths { get; set; }
    /// <summary> 获取指定类型角色属性在指定等级的养成数据 </summary>
    public double GetAttrGrowth(AttrType attrType, int level);
    /// <summary> 获取指定类型技能的养成数据 </summary>
    public Dictionary<string, SkillEffectData>? GetSkillEffectData(SkillType skillType);
}