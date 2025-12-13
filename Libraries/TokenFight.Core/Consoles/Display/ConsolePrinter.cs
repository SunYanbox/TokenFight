namespace TokenFight.Core.Consoles.Display;

/// <summary> 控制台输出助手 </summary>
public class ConsolePrinter
{
    /// <summary> 数据队列 </summary>
    public readonly Queue<OutputItem> OutputItems = new();

    /// <summary> 添加一条带颜色的文本信息(不会自动换行) </summary>
    public ConsolePrinter Add(string msg, ConsoleColor color = ConsoleColor.White)
    {
        OutputItems.Enqueue(new OutputItem
        {
            Text = msg,
            Color = color
        });
        return this;
    }

    /// <summary> 批量显示颜色格式化文本数据(不会自动换行) </summary>
    public void Display()
    {
        DisplayInfo(OutputItems);
        OutputItems.Clear();
    }

    /// <summary> 批量显示颜色格式化文本数据(不会自动换行) </summary>
    public static void DisplayInfo(Queue<OutputItem> outputRecords)
    {
        ConsoleColor beginColor = Console.ForegroundColor;
        ConsoleColor? currentColor = Console.ForegroundColor;

        while (outputRecords.TryDequeue(out OutputItem? date))
        {
            if (currentColor != date.Color)
            {
                currentColor = date.Color;
                Console.ForegroundColor = date.Color;
            }
            Console.Write(date.Text);
        }

        Console.ResetColor();
        Console.ForegroundColor = beginColor;
    }
}