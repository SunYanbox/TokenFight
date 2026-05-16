using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Entities.Masters;

public class ShieldMaster(IActor actor) : IShieldMaster
{
    public WeakReference<IActor> Owner { get; set; } = new(actor);

    private readonly Dictionary<string, double> _shields = new();

    public void Add(string id, double value)
    {
        _shields.Remove(id);
        _shields.Add(id, value);
    }

    public void Remove(string? id)
    {
        if (string.IsNullOrEmpty(id))
        {
            _shields.Clear();
            return;
        }
        _shields.Remove(id);
    }

    public void RemoveShields(IEnumerable<string> ids)
    {
        foreach (string id in new HashSet<string>(ids))
        {
            Remove(id);
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
            HashSet<string> shouldRemove = [];
            // 更新护盾
            foreach (string id in _shields.Keys)
            {
                _shields[id] -= damage;
                if (_shields[id] <= 0)
                {
                    shouldRemove.Add(id);
                }
            }
            // 移除无效护盾
            if (shouldRemove.Count > 0)
            {
                foreach (string id in shouldRemove)
                {
                    _shields.Remove(id);
                }
            }
        }
    }

    public double GetShield(string id) => _shields?.GetValueOrDefault(id, 0) ?? 0;
}
