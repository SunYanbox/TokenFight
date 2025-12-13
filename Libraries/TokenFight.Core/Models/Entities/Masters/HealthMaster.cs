using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Interfaces.Attrs;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Entities.Masters;

/// <summary>
/// 生命值系统
///
/// 绑定一个AttrSet用于跟踪最大生命值变化
/// </summary>
public class HealthMaster: IHealthMaster
{
    public required WeakReference<IActor> Owner { get; set; }
    private double MaxHealth { get; set; }
    private double CurrentHealth { get; set; }
    private IAttrSet? AttrSet { get; set; }

    [SetsRequiredMembers]
    public HealthMaster(IActor owner)
    {
        Owner = new WeakReference<IActor>(owner);
        AttrSet = owner.AttrSet;
        MaxHealth = (AttrSet?.GetBaseAttr(AttrType.Health) ?? 0)
                    + (AttrSet?.GetGainAttr(AttrType.Health) ?? 0);
        MaxHealth = Math.Max(MaxHealth, 1);
        CurrentHealth = MaxHealth;
    }

    public double Health
    {
        get
        {
            UpdateMaxHealth();
            CurrentHealth = Math.Max(Math.Min(CurrentHealth, MaxHealth), 0);
            return CurrentHealth;
        }
    }

    public double HealthMax
    {
        get
        {
            UpdateMaxHealth();
            return MaxHealth;
        }
    }

    private void UpdateMaxHealth()
    {
        double maxHealth = (AttrSet?.GetBaseAttr(AttrType.Health) ?? 0)
                           + (AttrSet?.GetGainAttr(AttrType.Health) ?? 0);
        // 最大生命值变化
        if (Math.Abs(maxHealth - MaxHealth) >= double.Epsilon)
        {
            double change = maxHealth / Math.Max(MaxHealth, 1);
            CurrentHealth *= change;
            MaxHealth = maxHealth;
        }
    }

    public void TakeDamage(double damage, out double overflowDamage)
    {
        UpdateMaxHealth();
        overflowDamage = 0;
        double health = Health;
        if (health >= damage)
        {
            CurrentHealth -= damage;
        }
        else
        {
            overflowDamage = damage - health;
            CurrentHealth = 0;
        }
    }

    public void TakeHeal(double heal, out double overflowHeal)
    {
        UpdateMaxHealth();
        overflowHeal = 0;
        double health = Health;
        if (health + heal > MaxHealth)
        {
            overflowHeal = health + heal - MaxHealth;
            CurrentHealth = MaxHealth;
        }
        else
        {
            CurrentHealth += heal;
        }
    }
}