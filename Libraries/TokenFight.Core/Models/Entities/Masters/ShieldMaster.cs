using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Entities.Masters;

public class ShieldMaster(IActor actor) : IShieldMaster
{
    public WeakReference<IActor> Owner { get; set; } = new(actor);
    
    private readonly Dictionary<string, double> _shields = new();

    public void Add(string Id, double value)
    {
        _shields.Remove(Id);
        _shields.Add(Id, value);
    }

    public void Remove(string? Id)
    {
        if (string.IsNullOrEmpty(Id))
        {
            _shields.Clear();
            return;
        }
        _shields.Remove(Id);
    }
    
    public void RemoveShields(IEnumerable<string> Ids)
    {
        foreach (var Id in new HashSet<string>(Ids))
        {
            Remove(Id);
        }
    }
    
    public double Shield => _shields.Count > 0 ? _shields.Values.Max() : 0;
    
    public void TakeDamage(double damage, out double residualDamage, out double shieldDefense)
    {
        double maxShield = Shield;
        if (damage >= maxShield)
        {
            residualDamage = damage - maxShield;
            shieldDefense = maxShield;
            Remove(null); // 清理所有护盾
        }
        else
        {
            residualDamage = 0;
            shieldDefense = damage;
            HashSet<string> shouldRemove = new();
            // 更新护盾
            foreach (string Id in _shields.Keys)
            {
                _shields[Id] -= damage;
                if (_shields[Id] <= 0)
                {
                    shouldRemove.Add(Id);
                }
            }
            // 移除无效护盾
            if (shouldRemove.Count > 0)
            {
                foreach (string Id in shouldRemove)
                {
                    _shields.Remove(Id);
                }
            }
        }
    }
    
    public double GetShield(string Id)
    {
        return _shields?.GetValueOrDefault(Id, 0) ?? 0;
    }
}