namespace TokenFight.Core.Models.Effects.Effects;

/// <summary> 回合开始时结算效果生命周期 </summary>
public abstract class RoundBeginEffect : BaseEffect
{
    public new void OnRoundBegin()
    {
        LifeCycle?.SettlementCycle();
    }

    public new void OnRoundEnd()
    {

    }
}