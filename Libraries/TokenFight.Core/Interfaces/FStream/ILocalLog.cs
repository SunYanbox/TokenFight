using System.Text;
using TokenFight.Core.Enums.FStream;
using TokenFight.Core.Interfaces.Bases;

namespace TokenFight.Core.Interfaces.FStream;

public interface ILocalLog : ISystem
{
    protected delegate void LogFunc(string msg);

    /// <summary> 运行一个任务, 静默处理错误 </summary>
    public bool TryCatch(string task, Func<bool> func);
    protected void LocalLogMsg(LocalLogType type, string message);

    /// <summary> 记录一条本地日志信息 </summary>
    public void Info(string message);

    /// <summary> 记录一条本地日志信息 </summary>
    public void Warn(string message);

    /// <summary> 记录一条本地日志信息 </summary>
    public void LogError(string message);

    /// <summary> 记录一条本地日志信息 </summary>
    public void Debug(string message);

    /// <summary> 记录一条战斗详情信息 </summary>
    public void Detail(string message);

    /// <summary> 记录一条战斗概要信息 </summary>
    public void Summary(string message);

    /// <summary> 记录一条统计信息 </summary>
    public void Stats(string message);
    /// <summary>
    /// 限制每行最多140字符
    /// </summary>
    /// <param name="input"></param>
    /// <param name="maxLength"></param>
    private static string SplitToLinesWithWords(string input, int maxLength = 140)
    {
        if (string.IsNullOrEmpty(input)) return input;

        List<string> lines = [];
        string[] words = input.Split(' ');
        var currentLine = new StringBuilder();

        foreach (string word in words)
        {
            if (currentLine.Length + word.Length + 1 <= maxLength) // +1 是空格
            {
                if (currentLine.Length > 0)
                    currentLine.Append(' ');
                currentLine.Append(word);
            }
            else
            {
                if (currentLine.Length > 0)
                    lines.Add(currentLine.ToString());

                currentLine = new StringBuilder(word);
            }
        }

        if (currentLine.Length > 0)
            lines.Add(currentLine.ToString());

        return string.Join(Environment.NewLine + "  ", lines);
    }
}