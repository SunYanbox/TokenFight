using TokenFight.Core.Constants;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Interfaces.Attrs;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Entities.Masters;

public class ActionValueMaster: IActionValueMaster
{
    public WeakReference<IActor> Owner { get; set; }
    private double Speed { get; set; }
    private double ActionDistance { get; set; }
    private IAttrSet? AttrSet { get; set; }

    public ActionValueMaster(IActor actor)
    {
        Owner = new WeakReference<IActor>(actor);
        AttrSet = actor.AttrSet;
        ActionDistance = GameConst.DefaultActionDistance;
        Speed = AttrSet.GetAttr(AttrType.Speed);
    }

    /// <summary> 获取当前行动值 </summary>
    public double ActionValue
    {
        get
        {
            Speed = AttrSet?.GetAttr(AttrType.Speed) ?? 1;
            if (Speed < 1) return Double.MaxValue;
            return ActionDistance / Speed;
        }
    }

    /// <summary> 根据速度重置行动值 </summary>
    public void Reset()
    {
        ActionDistance = GameConst.DefaultActionDistance;
    }

    /// <summary> 行动指定数值的行动值 </summary>
    public void Action(double actionValue)
    {
        if (actionValue < 0) return;
        ActionDistance -= actionValue * Speed;
        ActionDistance = Math.Max(0, ActionDistance);
    }
    
    /// <summary> 按照percent拉条或推条, percent>0时推条, percent小于0时拉条 </summary>
    public void Push(double percent)
    {
        if (percent > 0) ActionDistance *= 1 + percent;
        else if (percent < 0)
        {
            ActionDistance *= 1 + Math.Max(-1, percent);
            ActionDistance = Math.Max(0, ActionDistance);
        }
    }
}