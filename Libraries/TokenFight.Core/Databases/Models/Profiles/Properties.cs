using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Effects;

namespace TokenFight.Core.Databases.Models.Profiles;

/// <summary> 属性 </summary>
public class Properties
{
    /// <summary> 模板ID </summary>
    public required string Id { get; set; }
    /// <summary> 模板类型 </summary>
    public required ItemType Type { get; set; }
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
    public EntryValue? MainEntry { get; set; }
    /// <summary> 遗器副词条与其随机数值 </summary>
    public List<EntryValue>? SubEntries { get; set; }

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

    public Properties() { }

    /// <summary>
    /// 复制构造函数 - 创建当前实例的深拷贝
    /// </summary>
    /// <param name="source">要复制的源对象</param>
    [SetsRequiredMembers]
    public Properties(Properties source)
    {
        ArgumentNullException.ThrowIfNull(source);

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
        if (source.WeaponLayers is not null) WeaponLayers = new LevelProgress(source.WeaponLayers);
        if (source.WeaponLevel is not null) WeaponLevel = new LevelProgress(source.WeaponLevel);
        if (source.RelicsLevel is not null) RelicsLevel = new LevelProgress(source.RelicsLevel);
        if (source.ActorLevel is not null) ActorLevel = new LevelProgress(source.ActorLevel);

        // 复制枚举类型
        MainEntry = new EntryValue(source.MainEntry);

        // 深拷贝列表
        if (source.SubEntries != null)
        {
            SubEntries = new List<EntryValue>(source.SubEntries);
        }

        // 深拷贝字典
        if (source.SkillLevel != null)
        {
            SkillLevel = new Dictionary<SkillType, Dictionary<string, LevelProgress>>();
            foreach (KeyValuePair<SkillType, Dictionary<string, LevelProgress>> skillPair in source.SkillLevel)
            {
                Dictionary<string, LevelProgress> innerDict = new();
                foreach (KeyValuePair<string, LevelProgress> levelPair in skillPair.Value)
                {
                    innerDict[levelPair.Key] = new LevelProgress(levelPair.Value);
                }
                SkillLevel[skillPair.Key] = innerDict;
            }
        }
    }

    /// <summary> 覆盖当前实例的属性 | 仅通过source中非空属性覆盖 </summary>
    public void OverrideProperties(Properties source)
    {
        if (source == null || source.Id != Id)
            throw new ArgumentNullException(nameof(source));

        // 复制值类型和不可变引用类型
        if (source.Name is not null) Name = source.Name;
        if (source.Desc is not null) Desc = source.Desc;
        if (source.SkillId is not null) SkillId = source.SkillId;
        if (source.RelicsTypeId is not null) RelicsTypeId = source.RelicsTypeId;
        if (source.ActorId is not null) ActorId = source.ActorId;
        if (source.GiftCode is not null) GiftCode = source.GiftCode;
        if (source.UseCallback is not null) UseCallback = source.UseCallback;

        // 复制可空值类型
        if (source.GiftMaxExchangeTimes.HasValue) GiftMaxExchangeTimes = source.GiftMaxExchangeTimes;
        if (source.CurrentResources.HasValue) CurrentResources = source.CurrentResources;
        if (source.MaxResources.HasValue) MaxResources = source.MaxResources;

        // 复制引用类型 - 值类型成员不需要深拷贝
        if (source.WeaponLayers is not null) WeaponLayers = new LevelProgress(source.WeaponLayers);
        if (source.WeaponLevel is not null) WeaponLevel = new LevelProgress(source.WeaponLevel);
        if (source.RelicsLevel is not null) RelicsLevel = new LevelProgress(source.RelicsLevel);
        if (source.ActorLevel is not null) ActorLevel = new LevelProgress(source.ActorLevel);

        // 复制枚举类型
        if (source.MainEntry is not null) MainEntry = new EntryValue(source.MainEntry);

        // 深拷贝列表
        if (source.SubEntries != null)
        {
            SubEntries = new List<EntryValue>(source.SubEntries);
        }

        // 深拷贝字典
        if (source.SkillLevel != null)
        {
            SkillLevel = new Dictionary<SkillType, Dictionary<string, LevelProgress>>();
            foreach (KeyValuePair<SkillType, Dictionary<string, LevelProgress>> skillPair in source.SkillLevel)
            {
                Dictionary<string, LevelProgress> innerDict = new();
                foreach (KeyValuePair<string, LevelProgress> levelPair in skillPair.Value)
                {
                    innerDict[levelPair.Key] = new LevelProgress(levelPair.Value);
                }
                SkillLevel[skillPair.Key] = innerDict;
            }
        }
    }

}