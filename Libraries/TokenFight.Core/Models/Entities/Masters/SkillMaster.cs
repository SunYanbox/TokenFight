using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Entities.Masters;

public class SkillMaster : ISkillMaster
{
    public required WeakReference<IActor> Owner { get; set; }
    private readonly Dictionary<string, ISkill> _skills = new();
    private IActor? _defaultTarget;

    public bool ContainsKey(string id) => _skills.ContainsKey(id);

    public void UnsubscriptAll()
    {
        foreach (ISkill skill in _skills.Values)
        {
            if (skill.PassiveData?.Empty ?? true) continue;
            skill.PassiveData.Unsubscribe();
        }
    }

    public ISkillMaster SetTarget(IActor target)
    {
        if (!target.IsActive)
        {
            throw new ArgumentException("无法设置将要被清除引用的成员为技能目标");
        }
        _defaultTarget = target;
        return this;
    }

    public void Add(ISkill skill)
    {
        if (string.IsNullOrEmpty(skill.Id)) return;
        if (!skill.PassiveData?.Empty ?? false)
        {
            skill.PassiveData.Subscribe();
        }
        _skills.Add(skill.Id, skill);
    }

    public void Remove(string id)
    {
        if (_skills.TryGetValue(id, out ISkill? skill))
        {
            skill.PassiveData?.Unsubscribe();
        }

        _skills.Remove(id);
    }

    public ISkill GetSkill(string id)
    {
        ISkillMaster.SetSkillTarget(_skills[id], _defaultTarget);
        return _skills[id];
    }

    public bool HasSkill(string id) => _skills.ContainsKey(id);

    public IEnumerable<ISkill> GetActiveSkills()
    {
        List<ISkill> result = [];
        foreach (ISkill skill in _skills.Values)
        {
            ISkillMaster.SetSkillTarget(skill, _defaultTarget);
            if (skill.CanUse() && (skill.PassiveData?.Empty ?? true))
            {
                result.Add(skill);
            }
        }
        return result;
    }

    public IEnumerable<ISkill> GetActiveSkillsApartFromUltimate()
    {
        List<ISkill> result = [];
        foreach (ISkill skill in _skills.Values)
        {
            ISkillMaster.SetSkillTarget(skill, _defaultTarget);
            if (skill.CanUse() && (skill.PassiveData?.Empty ?? true))
            {
                if (skill is IUltimateSkill) continue;
                result.Add(skill);
            }
        }
        return result;
    }

    public IEnumerable<ISkill> GetPassiveSkills()
    {
        List<ISkill> result = [];
        foreach (ISkill skill in _skills.Values)
        {
            ISkillMaster.SetSkillTarget(skill, _defaultTarget);
            if (skill.CanUse() && (!skill.PassiveData?.Empty ?? false))
            {
                result.Add(skill);
            }
        }
        return result;
    }

    public IEnumerable<ISkill> GetSkillAll() => _skills.Values;
    public void ClearRef()
    {
        _defaultTarget = null;
        foreach (ISkill skill in _skills.Values)
        {
            skill.Source = null!;
        }

        foreach (ISkill skill in _skills.Values)
        {
            if (!(skill.PassiveData?.Empty ?? true))
            {
                skill.PassiveData.Unsubscribe();
            }
        }
        _skills?.Clear();
    }
}
