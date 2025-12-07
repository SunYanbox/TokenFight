using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Effects.Effects;

/// <summary> 基础效果 </summary>
public abstract class BaseEffect: IEffect
{
    public required string Id { get; set; }
    public EffectType Type { get; set; }
    public required WeakReference<IActor> Source { get; set; }
    public required WeakReference<IActor> Target { get; set; }
    public required PassiveData PassiveCallbackData { get; set; }
    public ILifeCycleMaster? LifeCycle { get; set; }
    public virtual void OnApply()
    {
        PassiveCallbackData.Subscribe();
    }
    
    public virtual void OnReapply(IEffect newEffect)
    {
        if ((LifeCycle?.HasDuration ?? false) && (newEffect.LifeCycle?.HasDuration ?? false))
        {
            LifeCycle.CurrentDuration = newEffect.LifeCycle.CurrentDuration;
        }
        if ((LifeCycle?.HasStack ?? false) && (newEffect.LifeCycle?.HasStack ?? false))
        {
            LifeCycle.CurrentStack += newEffect.LifeCycle.CurrentStack;
        }
        if ((LifeCycle?.HasMark ?? false) && (newEffect.LifeCycle?.HasMark ?? false))
        {
            LifeCycle.CurrentMark += newEffect.LifeCycle.CurrentMark;
        }
    }

    public virtual void OnRemove()
    {
        PassiveCallbackData.Unsubscribe();
    }
    public virtual void OnRoundBegin()
    {
        LifeCycle?.SettlementCycle();
    }
    public virtual void OnRoundEnd()
    {
        
    }
}