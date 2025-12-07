using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models.Actions;
using TokenFight.Core.Models.Entities.Masters;

namespace TokenFight.Core.Models.Effects.Effects;

/// <summary> 限时的护盾效果 | 回合开始时结算 </summary>
public class BaseShieldTimeLimited: BaseEffect
{
    /// <summary> 可以在外部设置护盾的值 </summary>
    public double Shield { get; set; }
    /// <summary> 是否可叠加 </summary>
    public bool Stackable { get; protected set; }
    
    [SetsRequiredMembers]
    public BaseShieldTimeLimited(IActor source, IActor target, string id, double shield, int duration, bool stackable,
        GameSystemRegistry gameSystemRegistry)
    {
        Id = id;
        Type = EffectType.Buff;
        Source = new WeakReference<IActor>(source);
        Target = new WeakReference<IActor>(target);
        PassiveCallbackData = new PassiveData(gameSystemRegistry.EventSystem, gameSystemRegistry.LocalLog);
        LifeCycle = new LifeCycleMaster
        {
            Owner = new WeakReference<IActor>(target)
        };
        LifeCycle.InitDuration(duration);
        Stackable = stackable;
        Shield = shield;
    }

    public override void OnApply()
    {
        IActor? source = ActorHelper.GetActorFromWeakRef(Source);
        double shield = Shield;
        if (Target.TryGetTarget(out IActor? target) && source != null)
        {
            BeforeExcuteContext? beforeExcuteContext 
                = EventHelper.TriggerBeforeExcuteContext(EventType.ShieldBefore, source, target, shield, null);
            shield = beforeExcuteContext?.ValueCtx ?? shield;
            if (Stackable)
            {
                shield += target.ShieldMaster!.GetShield(Id);
            }
            target.ShieldMaster.Add(Id, shield);
            EventHelper.TriggerShieldContext(source, target, shield);
        }
    }
    
    public override void OnRemove()
    {
        if (Target.TryGetTarget(out IActor? target))
        {
            target.ShieldMaster.Remove(Id);
        }
    }

    public override void OnReapply(IEffect newEffect)
    {
        if (newEffect is BaseShieldTimeLimited)
        {
            if ((LifeCycle?.HasDuration ?? false) && newEffect.LifeCycle.HasDuration)
            {
                LifeCycle.CurrentDuration = newEffect.LifeCycle.CurrentDuration;
            }

            OnApply();
        }
    }
}