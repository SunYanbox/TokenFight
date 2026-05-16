using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models.Entities.Masters;

namespace TokenFight.Core.Models.Effects.Effects;

/// <summary> 标记效果 | 无限持续时间的标记效果 | 每次应用时标记次数+1 </summary>
public class BaseEffectMark : BaseEffect
{
    [SetsRequiredMembers]
    public BaseEffectMark(IActor source, IActor target, string id,
        GameSystemRegistry gameSystemRegistry)
    {
        Id = id;
        Type = EffectType.Mark;
        Source = new WeakReference<IActor>(source);
        Target = new WeakReference<IActor>(target);
        PassiveCallbackData = new PassiveData(gameSystemRegistry.EventSystem, gameSystemRegistry.LocalLog);
        LifeCycle = new LifeCycleMaster
        {
            Owner = new WeakReference<IActor>(target)
        };
        LifeCycle.InitMark(1);
    }

    public override void OnReapply(IEffect newEffect)
    {
        LifeCycle!.CurrentMark += newEffect.LifeCycle.CurrentMark;
    }
    public override void OnRoundBegin()
    {

    }

    public override void OnRoundEnd()
    {

    }
}