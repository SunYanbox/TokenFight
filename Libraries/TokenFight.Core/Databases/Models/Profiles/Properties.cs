using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Effects;

namespace TokenFight.Core.Databases.Models.Profiles;

/// <summary> 属性 </summary>
public struct Properties
{
    /// <summary> 名称 </summary>
    public string? Name { get; set; }
    /// <summary> 描述 </summary>
    public string? Desc { get; set; }
    
    /// <summary> 光锥技能Id </summary>
    public string? SkillId { get; set; }
    /// <summary> 光锥叠影层数 </summary>
    public LevelProgress? WeaponLayers { get; set; }
    /// <summary> 光锥等级 </summary>
    public LevelProgress? WeaponLevel { get; set; }
    
    /// <summary> 遗器类型Id </summary>
    public string? RelicsTypeId { get; set; }
    /// <summary> 遗器等级 </summary>
    public LevelProgress? RelicsLevel { get; set; }
    /// <summary> 遗器主词条 </summary>
    public AttrType? MainEntry { get; set; }
    /// <summary> 遗器副词条与强化等级 </summary>
    public List<(AttrType, int)>? SubEntries { get; set; }
    
    /// <summary> 角色Id </summary>
    public string? ActorId { get; set; }
    /// <summary> 角色等级 </summary>
    public LevelProgress? ActorLevel { get; set; }
    /// <summary> 不同技能的角色等级 </summary>
    public Dictionary<SkillType, Dictionary<string, LevelProgress>>? SkillLevel { get; set; }
    
    
    
    
    
}