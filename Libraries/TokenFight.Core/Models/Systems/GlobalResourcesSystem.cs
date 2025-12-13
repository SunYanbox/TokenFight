using TokenFight.Core.Interfaces.FStream;
using TokenFight.Core.Interfaces.Systems;

namespace TokenFight.Core.Models.Systems;

public class GlobalResourcesSystem(ILocalLog localLog): IGlobalResourcesSystem
{
    public void Init()
    {
        MaxSkillPoint = 5;
        SkillPoint = (int)Math.Floor(MaxSkillPoint * 0.6);
    }

    public void Reset()
    {
        Init();
    }

    public int SkillPoint { get; set; }
    public int MaxSkillPoint { get; set; }
    public double ActionValue { get; set; }

    public void AdjustSkillPoint(int delta = 0)
    {
        int oldSkillPoint = SkillPoint;
        SkillPoint += delta;
        SkillPoint = Math.Max(Math.Min(SkillPoint, MaxSkillPoint), 0);
        if (delta != 0) localLog.Debug($"[全局资源] 战技点已变更: ({oldSkillPoint} -> {SkillPoint})/{MaxSkillPoint}");
    }
    public void AdjustActionValue(double delta = 0)
    {
        double oldSkillPoint = ActionValue;
        ActionValue += delta;
        ActionValue = Math.Max(ActionValue, 0);
        if (Math.Abs(delta) < double.Epsilon) localLog.Debug($"[全局资源] 行动值已变更: {oldSkillPoint} -> {ActionValue}");
    }
}