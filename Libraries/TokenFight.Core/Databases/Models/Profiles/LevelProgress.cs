namespace TokenFight.Core.Databases.Models.Profiles;

/// <summary> 当前等级与最大等级 | 养成信息 </summary>
public struct LevelProgress
{
    public int Level { get; set; }
    public int MaxLevel { get; set; }
}