using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models.Attrs;
using TokenFight.Core.Models.Entities.Masters;

namespace TokenFight.Core.Models.Effects.Effects;

/// <summary> 封装百分比增益 可以按照层数等比例提升 </summary>
public class BaseEffectPctGain : BaseEffect
{
    private readonly AttrModifyData _modifyData;

    [SetsRequiredMembers]
    public BaseEffectPctGain(IActor source, IActor target, string id, Dictionary<int, double> modifyData,
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
        _modifyData = new AttrModifyData
        {
            Id = id,
            Type = AttrModifyType.Percent,
            ModifyData = modifyData,
            IsTemp = false
        };
    }

    public void InitDuration(int duration)
    {
        LifeCycle!.InitDuration(duration);
    }

    public void InitStack(int stack, int maxStack, int deltaStack = -1)
    {
        LifeCycle!.InitStack(stack, maxStack, deltaStack);
    }

    public void InitMark(int mark = 1)
    {
        LifeCycle!.InitMark(mark);
    }

    public override void OnApply()
    {
        if (Target.TryGetTarget(out IActor? target))
        {
            Dictionary<int, double> modifyData = new(_modifyData.ModifyData);
            foreach (int key in modifyData.Keys)
            {
                modifyData[key] *= LifeCycle?.HasStack ?? false ? LifeCycle.CurrentStack ?? 1 : 1;
            }
            target.AttrSet.SetAttr(new AttrModifyData
            {
                Id = _modifyData.Id,
                Type = _modifyData.Type,
                ModifyData = modifyData,
                IsTemp = _modifyData.IsTemp
            });
        }
    }

    public override void OnRemove()
    {
        if (Target.TryGetTarget(out IActor? target))
        {
            target.AttrSet.Remove(_modifyData.Id);
        }
    }
}