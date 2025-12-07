namespace TokenFight.Core.Interfaces.Entities.Masters;

public interface IHealthMaster: IMaster
{
    /// <summary> 获取当前生命值 </summary>
    public double Health { get; }
    /// <summary> 获取当前最大生命值 </summary>
    public double HealthMax { get; }

    /// <summary> 造成伤害, 并输出溢出伤害 </summary>
    /// <remarks> 这个函数内部不应该发布事件 </remarks>
    /// <param name="damage">需要造成的伤害</param>
    /// <param name="overflowDamage">溢出伤害</param>
    public void TakeDamage(double damage, out double overflowDamage);

    /// <summary> 提供治疗, 并输出溢出治疗量 </summary>
    /// <remarks> 这个函数内部不应该发布事件 </remarks>
    public void TakeHeal(double heal, out double overflowHeal);
}