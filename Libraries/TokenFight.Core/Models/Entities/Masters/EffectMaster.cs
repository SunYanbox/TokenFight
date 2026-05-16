using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Entities.Masters;

public class EffectMaster(IActor actor) : IEffectMaster
{
    public WeakReference<IActor> Owner { get; set; } = new(actor);
    private readonly Dictionary<string, IEffect> _effects = new();
    private readonly Dictionary<EffectType, HashSet<string>> _existBuffs = new()
    {
        { EffectType.Buff, [] },
        { EffectType.Debuff, [] },
        { EffectType.Mark, [] }
    };

    public HashSet<string> GetEffectIds(EffectType type) => [.. _existBuffs[type]];

    public void RoundBegin()
    {
        foreach (IEffect effect in _effects.Values)
            effect.OnRoundBegin();
        foreach (IEffect effect in _effects.Values.ToArray())
        {
            if (!(effect.LifeCycle?.IsInvalid() ?? false)) continue;
            Remove(effect.Id);
            _existBuffs[effect.Type].Remove(effect.Id);
        }
    }

    public void RoundEnd()
    {
        foreach (IEffect effect in _effects.Values)
            effect.OnRoundEnd();
        foreach (IEffect effect in _effects.Values.ToArray())
        {
            if (!(effect.LifeCycle?.IsInvalid() ?? false)) continue;
            Remove(effect.Id);
            _existBuffs[effect.Type].Remove(effect.Id);
        }
    }

    public void Apply(IEffect effect)
    {
        if (string.IsNullOrEmpty(effect.Id)) return;
        effect.OnApply();
        if (_effects.TryGetValue(effect.Id, out IEffect? effect1))
        {
            effect1.OnReapply(effect);
            TriggerEvent(EventType.EffectApply, effect);
        }
        else
        {
            _effects.Add(effect.Id, effect);
            _existBuffs[effect.Type].Add(effect.Id);
            TriggerEvent(EventType.EffectApply, effect);
        }
    }

    public void Remove(string id)
    {
        if (_effects.TryGetValue(id, out IEffect? effect))
        {
            TriggerEvent(EventType.EffectRemove, effect);
            effect.OnRemove();
            _effects.Remove(id);
            _existBuffs[effect.Type].Remove(id);
        }
    }

    public void Remove(IEnumerable<string> ids)
    {
        foreach (string id in ids)
        {
            Remove(id);
        }
    }

    public bool Has(string id) => _effects.ContainsKey(id);
    public IEffect Get(string id) => _effects[id];
    /// <summary> 获取所有效果 </summary>
    public IEnumerable<IEffect> Values => _effects.Values;
    /// <summary> 是否无任何效果 </summary>
    public bool Empty => _effects.Count == 0;
    /// <summary> 触发效果相关事件 </summary>
    /// <param name="type"></param>
    /// <param name="effect"></param>
    private static void TriggerEvent(EventType type, IEffect effect)
    {
        if (type != EventType.EffectRemove && type != EventType.EffectApply) return;
        EventHelper.TriggerEffectContext(effect, type == EventType.EffectApply);
    }

    /// <summary> 清理来自指定成员的所有引用 </summary>
    public void RemoveWithActor(IActor actor)
    {
        foreach ((_, IEffect effect) in _effects.AsReadOnly())
        {
            if (effect.Source.TryGetTarget(out IActor? target) && target == actor)
            {
                Remove(effect.Id);
                _existBuffs[effect.Type].Remove(effect.Id);
            }
        }
    }
}