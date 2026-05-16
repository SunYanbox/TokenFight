using TokenFight.Core.Interfaces.Bases;

namespace TokenFight.Core.Interfaces.Systems;

public interface IGlobalResourcesSystem : ISystem
{
    /// <summary> 战技点 </summary>
    public int SkillPoint { get; protected set; }
    /// <summary> 战技点上限 </summary>
    public int MaxSkillPoint { get; protected set; }
    /// <summary> 行动值 </summary>
    public double ActionValue { get; protected set; }

    /// <summary> 调整战技点 </summary>
    public void AdjustSkillPoint(int delta = 0);

    /// <summary> 调整全局行动值 </summary>
    public void AdjustActionValue(double delta = 0);
}