using System.Text;
using TokenFight.Core.Constants;
using TokenFight.Core.Enums.FStream;
using TokenFight.Core.Interfaces.FStream;

namespace TokenFight.Core.Models.FStream;

public class LocalLog : ILocalLog
{
    public void Init()
    {
        string pathToMod = Environment.CurrentDirectory;

        TryCatch("初始化本地日志核心", () =>
        {
            const int maxLogFileSize = 10 * 1024 * 1024; // 10 MB
            LogFolderPath = Path.Combine(pathToMod, GameConst.LogFolder);
            DataFolderPath = Path.Combine(pathToMod, GameConst.DataFolder);

            TryCatch("创建日志文件夹", () =>
            {
                Directory.CreateDirectory(LogFolderPath);
                return true;
            });
            TryCatch("创建数据文件夹", () =>
            {
                Directory.CreateDirectory(DataFolderPath);
                return true;
            });

            string infoPath = Path.Combine(LogFolderPath, "info.log");
            string warnPath = Path.Combine(LogFolderPath, "warn.log");
            string debugPath = Path.Combine(LogFolderPath, "debug.log");
            string errorPath = Path.Combine(LogFolderPath, "error.log");
            string fightDetailPath = Path.Combine(LogFolderPath, "details.log");
            string fightSummaryPath = Path.Combine(LogFolderPath, "summary.md");
            string statsPath = Path.Combine(LogFolderPath, "stats.log");

            TryCatch("日志过大检测", () =>
            {
                foreach (string filePath in new[]
                         {
                             infoPath, warnPath, debugPath, errorPath,
                             fightDetailPath, fightSummaryPath,
                             statsPath
                         })
                {
                    if (File.Exists(filePath))
                    {
                        var fileInfo = new FileInfo(filePath);
                        if (fileInfo.Length > maxLogFileSize)
                        {
                            TryCatch($"日志文件过大，删除并重建: {filePath}", () =>
                            {
                                File.Delete(filePath);
                                return true;
                            });
                        }
                    }
                }
                return true;
            });

            RegisterWriterStream("注册Info日志写入流", LocalLogType.Info, infoPath);
            RegisterWriterStream("注册Warn日志写入流", LocalLogType.Warn, warnPath);
            RegisterWriterStream("注册Debug日志写入流", LocalLogType.Debug, debugPath);
            RegisterWriterStream("注册Error日志写入流", LocalLogType.Error, errorPath);
            RegisterWriterStream("注册战斗详情日志写入流", LocalLogType.FightDetails, fightDetailPath);
            RegisterWriterStream("注册战斗概要日志写入流", LocalLogType.FightSummary, fightSummaryPath);
            RegisterWriterStream("注册统计信息日志写入流", LocalLogType.Stats, statsPath);
            return true;
        });

        foreach (ILocalLog.LogFunc func in new List<ILocalLog.LogFunc> { Info, Debug, Warn, LogError })
        {
            func("----------------------------------------本地化日志加载完成----------------------------------------");
        }
        foreach (ILocalLog.LogFunc func in new List<ILocalLog.LogFunc> { Detail })
        {
            func("\n----------------------------------------战斗开始----------------------------------------");
        }

        Summary("\n---\n## 战斗开始");
        foreach (ILocalLog.LogFunc func in new List<ILocalLog.LogFunc> { Stats })
        {
            func("\n- - - - - - - - - - - - - - - - - - - - 统计开始 - - - - - - - - - - - - - - - - - - - -");
        }
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public bool TryCatch(string task, Func<bool> func)
    {
        try
        {
            bool result = func();
            Debug($"任务{task}已结束, 结果: {result}");
            return result;
        }
        catch (Exception e)
        {
            LogError($"{e.Message}\n\t{e.StackTrace}");
            return false;
        }
    }

    public void LocalLogMsg(LocalLogType type, string message)
    {
        if (_logWriters.TryGetValue(type, out StreamWriter? writer))
        {
            try
            {
                lock (_logLock)
                {
                    using (var sw = new StreamWriter(writer.BaseStream, writer.Encoding, 1024, true) { AutoFlush = true })
                    {
                        message += type == LocalLogType.Debug ? "\n" : string.Empty;
                        sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {SplitToLinesWithWords(message)}");
                        sw.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[本地日志] 无法正确记录{type.ToString()}类型的日志:" +
                                  $"\n\t> error: {e.Message}" +
                                  $"\n\t> stack: {e.StackTrace}" +
                                  $"\n\t> message: {message}");
                throw;
            }
        }
    }

    public void Info(string message)
    {
        LocalLogMsg(LocalLogType.Info, message);
    }
    public void Warn(string message)
    {
        LocalLogMsg(LocalLogType.Warn, message);
    }
    public void LogError(string message)
    {
        LocalLogMsg(LocalLogType.Error, message);
    }
    public void Debug(string message)
    {
        LocalLogMsg(LocalLogType.Debug, message);
    }
    public void Detail(string message)
    {
        LocalLogMsg(LocalLogType.FightDetails, message);
    }
    public void Summary(string message)
    {
        LocalLogMsg(LocalLogType.FightSummary, message);
    }
    public void Stats(string message)
    {
        LocalLogMsg(LocalLogType.Stats, message);
    }

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

    private void RegisterWriterStream(string task, LocalLogType type, string path)
    {
        TryCatch(task, () =>
        {
            _logWriters[type] =
                new StreamWriter(new FileStream(path, FileMode.Append, FileAccess.Write));
            return true;
        });
    }

    /// <summary> data文件夹路径 </summary>
    public string? DataFolderPath { get; set; }
    /// <summary> logs文件夹路径 </summary>
    public string? LogFolderPath { get; set; }
    private readonly Dictionary<LocalLogType, StreamWriter> _logWriters = new();
    private readonly Lock _logLock = new();
}
