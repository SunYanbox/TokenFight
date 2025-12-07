using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Interfaces.FStream;

namespace TokenFight.Core.Models.Effects;

/// <summary> 被动技能回调的数据结构 </summary>
/// <param name="eventSystem">事件系统</param>
/// <param name="localLog">本地日志系统</param>
public class PassiveData(IEventSystem eventSystem, ILocalLog localLog)
{
    /// <summary> 技能管理的事件 -> 回调函数 <br /> 在外部初始化, 使用Subscribe后就不应该再进行任何更改 </summary>
    public Dictionary<EventType, EventCallback> Callbacks { get; } = new();
    /// <summary> 事件系统 </summary>
    private readonly IEventSystem _eventSystem  = eventSystem;
    /// <summary> 注册所有技能回调 </summary>
    public void Subscribe()
    {
        foreach ((EventType type, EventCallback callback) in Callbacks)
        {
            if (type == EventType.Null) continue;
            localLog.TryCatch($"注册{type.ToString()}类型技能回调: {callback}", () =>
            {
                _eventSystem.Subscribe(type, callback);
                return true;
            });
        }
    }
    /// <summary> 移除所有技能回调 </summary>
    public void Unsubscribe()
    {
        foreach ((EventType type, EventCallback callback) in Callbacks)
        {
            if (type == EventType.Null) continue;
            localLog.TryCatch($"注销{type.ToString()}类型技能回调: {callback}", () =>
            {
                _eventSystem.Unsubscribe(type, callback);
                return true;
            });
        }
    }
    /// <summary> 是否没有任何回调 </summary>
    public bool Empty => Callbacks.Count == 0;
}