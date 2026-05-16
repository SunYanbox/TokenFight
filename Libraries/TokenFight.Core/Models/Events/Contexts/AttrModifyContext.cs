using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Events;

namespace TokenFight.Core.Models.Events.Contexts;

/// <summary> 修改属性参数封装 </summary>
public struct AttrModifyContext : IContext
{
    public EventType Type { get; init; }
    public object Sender { get; init; }
    /// <summary> 属性的唯一键 </summary>
    public string Id;
    /// <summary> 更改目标 </summary>
    public AttrModifyType ModifyType;
    /// <summary> 属性类型 -> 值 </summary>
    public Dictionary<int, double> ModifyData;
    /// <summary> 是否是临时属性 </summary>
    public bool IsTemp;
}