using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Interfaces.Effects;

namespace TokenFight.Core.Interfaces.Entities.Masters;

public interface IEffectMaster: IMaster
{
    public bool Empty { get; }
    /// <summary> 处理效果回合开始 </summary>
    public void RoundBegin();

    /// <summary> 处理效果回合结束 </summary>
    public void RoundEnd();

    /// <summary> 新添一个效果并应用 </summary>
    public void Apply(IEffect effect);

    /// <summary> 移除指定Id的效果 </summary>
    public void Remove(string id);
    
    /// <summary> 移除多个Id的效果 </summary>
    public void Remove(IEnumerable<string> ids);
    
    /// <summary> 获取所有效果 </summary>
    public IEnumerable<IEffect> Values { get; }

    /// <summary> 获取指定类型所有效果Id </summary>
    public HashSet<string> GetEffectIds(EffectType type);

    /// <summary> 判断是否有指定Id的效果 </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool Has(string id);

    /// <summary> 获取指定Id的效果, 获取不到报错 </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IEffect Get(string id);
}