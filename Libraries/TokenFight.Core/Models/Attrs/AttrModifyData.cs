using TokenFight.Core.Enums.Attrs;

namespace TokenFight.Core.Models.Attrs;

/// <summary> 修改属性参数封装 </summary>
public struct AttrModifyData
{
    /// <summary> 属性的唯一键 </summary>
    public string Id;

    /// <summary> 更改目标 </summary>
    public AttrModifyType Type;

    /// <summary> 属性类型 -> 值 </summary>
    public Dictionary<int, double> ModifyData;

    /// <summary> 是否是临时属性 </summary>
    public bool IsTemp;
}
