using TokenFight.Core.Enums.Events;

namespace TokenFight.Core.Interfaces.Events;

/// <summary>
/// 事件系统上下文
/// </summary>
public interface IContext
{
    /// <summary>
    /// 事件类型
    /// </summary>
    EventType Type { get; init; }
    /// <summary>
    /// 事件发布者
    /// </summary>
    object Sender { get; init; }
}