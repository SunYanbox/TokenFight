namespace TokenFight.Core.Databases.Models.Dungeons;

/// <summary>
/// 副本收益等信息
/// </summary>
public class DungeonInfo
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Desc { get; set; }
    public required int BaseToken { get; set; }
    public required TokenRange PerRandomToken { get; set; }
    public required EnemyEntry[] EnemyPool { get; set; }
}
