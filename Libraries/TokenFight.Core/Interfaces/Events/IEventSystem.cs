using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Bases;

namespace TokenFight.Core.Interfaces.Events;

public delegate void EventCallback(IContext context);

/// <summary>
/// 事件系统接口
/// </summary>
public interface IEventSystem: ISystem
{
    /// <summary>
    /// 触发事件
    /// </summary>
    void Trigger(EventType eventType, IContext context);
    /// <summary>
    /// 注册事件回调
    /// </summary>
    bool Subscribe(EventType eventType, EventCallback callback);
    /// <summary>
    /// 注销事件回调
    /// </summary>
    bool Unsubscribe(EventType eventType, EventCallback callback);
}