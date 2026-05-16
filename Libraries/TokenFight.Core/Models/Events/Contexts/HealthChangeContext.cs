using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 血量变化上下文 </summary>
public struct HealthChangeContext : IContext
{
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 血量变化的单位 </summary>
    public IActor Actor;
    /// <summary> 血量变化数值 </summary>
    public double Delta;
    /// <summary> 血量变化数值相对于总血量的比值 </summary>
    public double DeltaRatio;
}
