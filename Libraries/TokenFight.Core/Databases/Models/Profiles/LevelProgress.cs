namespace TokenFight.Core.Databases.Models.Profiles;

/// <summary> 当前等级与最大等级 | 养成信息 </summary>
public struct LevelProgress
{
    public int Level { get; set; }
    public int MaxLevel { get; set; }

    public LevelProgress() => Level = MaxLevel = 1;

    public LevelProgress(int level, int maxLevel)
    {
        Level = level;
        MaxLevel = maxLevel;
    }

    public LevelProgress(LevelProgress? other)
    {
        if (other?.Level == null || other?.MaxLevel == null)
            throw new ArgumentNullException(nameof(other));
        Level = other?.Level ?? -1;
        MaxLevel = other?.MaxLevel ?? -1;
    }
}