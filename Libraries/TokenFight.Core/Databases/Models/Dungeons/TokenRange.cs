using System.Diagnostics.CodeAnalysis;

namespace TokenFight.Core.Databases.Models.Dungeons;

/// <summary>
/// 随机收益范围
/// </summary>
public class TokenRange
{
    public required int Min { get; set; }
    public required int Max { get; set; }
    [SetsRequiredMembers]
    public TokenRange() => Min = Max = 0;
    [SetsRequiredMembers]
    public TokenRange(int min, int max)
    {
        Min = min;
        Max = max;
    }

    public override string ToString() => $"{Min}~{Max}";
}