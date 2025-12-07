namespace TokenFight.Core.Models.Effects.Effects;

/// <summary> 回合结束时结算效果生命周期 </summary>
public class RoundEndEffect: BaseEffect
{
    public new void OnRoundBegin()
    {
        
    }
    
    public new void OnRoundEnd()
    {
        LifeCycle?.SettlementCycle();
    }
}