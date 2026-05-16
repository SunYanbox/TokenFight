using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Interfaces.Effects;

/// <summary>
/// 状态效果
/// </summary>
public interface IEffect
{
    /// <summary> 效果Id </summary>
    public string Id { get; set; }
    /// <summary> 效果类型 </summary>
    public EffectType Type { get; set; }
    /// <summary> 技能释放者 </summary>
    public WeakReference<IActor> Source { get; set; }
    /// <summary> 技能目标 </summary>
    public WeakReference<IActor> Target { get; set; }
    /// <summary> 生命周期 </summary>
    public ILifeCycleMaster LifeCycle { get; set; }
    void OnApply();
    void OnRemove();
    void OnRoundBegin();
    void OnRoundEnd();
    void OnReapply(IEffect newEffect);
}
