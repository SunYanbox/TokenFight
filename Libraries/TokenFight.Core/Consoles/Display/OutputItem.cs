namespace TokenFight.Core.Consoles.Display;

/// <summary> 这是封装控制台输出的记录 </summary>
public record OutputItem
{
    /// <summary> 要输出的文本 </summary>
    public required string Text { get; init; }

    /// <summary> 文本颜色 </summary>
    public ConsoleColor Color { get; init; } = default;
}
