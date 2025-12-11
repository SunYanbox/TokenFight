using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Effects;

namespace TokenFight.Core.Databases.Models.Profiles;

/// <summary> 属性 </summary>
public class Properties
{
    /// <summary> 模板ID </summary>
    public required string Id { get; set; }
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
    /// <summary> 遗器副词条与其随机数值 </summary>
    public List<(AttrType, double)>? SubEntries { get; set; }
    
    /// <summary> 角色Id </summary>
    public string? ActorId { get; set; }
    /// <summary> 角色等级 </summary>
    public LevelProgress? ActorLevel { get; set; }
    /// <summary> 不同技能的角色等级 </summary>
    public Dictionary<SkillType, Dictionary<string, LevelProgress>>? SkillLevel { get; set; }
    
    /// <summary> 物品资源当前值 </summary>
    public double? CurrentResources { get; set; }
    /// <summary> 物品资源最大值 </summary>
    public double? MaxResources { get; set; }
    
    /// <summary> 礼物兑换码 </summary>
    public string? GiftCode { get; set; }
    /// <summary> 礼物最大兑换次数 </summary>
    public int? GiftMaxExchangeTimes { get; set; }
    
    /// <summary> 使用物品回调 </summary>
    public string? UseCallback { get; set; }
    
    /// <summary>
    /// 复制构造函数 - 创建当前实例的深拷贝
    /// </summary>
    /// <param name="source">要复制的源对象</param>
    [SetsRequiredMembers]
    public Properties(Properties source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        // 复制值类型和不可变引用类型
        Id = source.Id;
        Name = source.Name;
        Desc = source.Desc;
        SkillId = source.SkillId;
        RelicsTypeId = source.RelicsTypeId;
        ActorId = source.ActorId;
        GiftCode = source.GiftCode;
        UseCallback = source.UseCallback;
        
        // 复制可空值类型
        GiftMaxExchangeTimes = source.GiftMaxExchangeTimes;
        CurrentResources = source.CurrentResources;
        MaxResources = source.MaxResources;
        
        // 复制引用类型 - 值类型成员不需要深拷贝
        WeaponLayers = new LevelProgress(source.WeaponLayers);
        WeaponLevel = new LevelProgress(source.WeaponLevel);
        RelicsLevel = new LevelProgress(source.RelicsLevel);
        ActorLevel = new LevelProgress(source.ActorLevel);
        
        // 复制枚举类型
        MainEntry = source.MainEntry;
        
        // 深拷贝列表
        if (source.SubEntries != null)
        {
            SubEntries = new List<(AttrType, double)>(source.SubEntries);
        }
        
        // 深拷贝字典
        if (source.SkillLevel != null)
        {
            SkillLevel = new Dictionary<SkillType, Dictionary<string, LevelProgress>>();
            foreach (var skillPair in source.SkillLevel)
            {
                var innerDict = new Dictionary<string, LevelProgress>();
                foreach (var levelPair in skillPair.Value)
                {
                    innerDict[levelPair.Key] = new LevelProgress(levelPair.Value);
                }
                SkillLevel[skillPair.Key] = innerDict;
            }
        }
    }
}