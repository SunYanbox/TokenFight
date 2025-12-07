using System.Text;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Interfaces.FStream;

namespace TokenFight.Core.Models.Utils.Systems;

/// <summary>
/// 用来生成唯一Id的工具类
/// </summary>
public class IdGenerateSystem(ILocalLog localLog)
{
    private readonly Dictionary<TeamType, Queue<int>> _releases = new()
    {
        { TeamType.Player, new Queue<int>() },
        { TeamType.Enemy, new Queue<int>() }
    };
    private int _nextPlayer = 1;
    private int _nextEnemy = 1;

    /// <summary> 获取指定阵营成员的新Id </summary>
    public string GetNewId(TeamType team)
    {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(team + "_");
        if (_releases.TryGetValue(team, out Queue<int>? queue))
        {
            if (queue.Count > 0)
            {
                stringBuilder.Append(queue.Dequeue());
                return stringBuilder.ToString();
            }
        }

        switch (team)
        {
            case TeamType.Player:
                stringBuilder.Append(_nextPlayer++);
                break;
            case TeamType.Enemy:
                stringBuilder.Append(_nextEnemy++);
                break;
        }
        return stringBuilder.ToString();
    }

    /// <summary> 释放一个Id </summary>
    public void ReleaseId(string Id)
    {
        const string namePlayer = nameof(TeamType.Player) + "_";
        const string nameEnemy = nameof(TeamType.Enemy) + "_";
        if (Id.StartsWith(namePlayer))
        {
            try
            {
                int release = Convert.ToInt32(Id.Replace(namePlayer, ""));
                _releases[TeamType.Player].Enqueue(release);
            }
            catch (FormatException e)
            {
                localLog.LogError($"[IdGenerateSystem.ReleaseId] 解析的 '{Id}' 格式错误, 无法成功释放: {e.Message}");
                throw;
            }
            catch (OverflowException e)
            {
                localLog.LogError($"[IdGenerateSystem.ReleaseId] 解析的 '{Id}' 超出整数范围, 无法成功释放: {e.Message}");
                throw;
            }
        }
        else if (Id.StartsWith(nameEnemy))
        {
            try
            {
                int release = Convert.ToInt32(Id.Replace(nameEnemy, ""));
                _releases[TeamType.Enemy].Enqueue(release);
            }
            catch (FormatException e)
            {
                localLog.LogError($"[IdGenerateSystem.ReleaseId] 解析的 '{Id}' 格式错误, 无法成功释放: {e.Message}");
                throw;
            }
            catch (OverflowException e)
            {
                localLog.LogError($"[IdGenerateSystem.ReleaseId] 解析的 '{Id}' 超出整数范围, 无法成功释放: {e.Message}");
                throw;
            }
        }
    }
}