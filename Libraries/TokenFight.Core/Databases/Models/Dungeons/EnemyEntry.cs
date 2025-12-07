namespace TokenFight.Core.Databases.Models.Dungeons;

/// <summary>
/// 敌人信息
/// </summary>
public class EnemyEntry
{
    public required string EnemyId { get; set; }
    public required int Level { get; set; }
}