using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Consoles.Display;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Interfaces.Attrs;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models.Attrs;
using TokenFight.Core.Models.Effects.Effects;

namespace TokenFight.Core.Models.Entities.Actors;

/// <summary>
/// 玩家角色自带5%暴击率与50%暴击伤害加成
/// </summary>
public abstract class PlayerActor : BaseActor, IPlayer
{
    #region 默认键
    /// <summary> 默认普攻Id </summary>
    public string BasicAttackId => Id + nameof(SkillType.BasicAttack);
    /// <summary> 默认战技Id </summary>
    public string FightSkillId => Id + nameof(SkillType.FightSkill);
    /// <summary> 默认追加攻击Id </summary>
    public string FollowUpAttackId => Id + nameof(SkillType.FollowUpAttack);
    /// <summary> 默认终结技Id </summary>
    public string UltimateSkillId => Id + nameof(SkillType.UltimateSkill);
    /// <summary> 默认天赋Id </summary>
    public string NaturalTalentId => Id + nameof(SkillType.NaturalTalent);
    #endregion

    [SetsRequiredMembers]
    protected PlayerActor(GameSystemRegistry gameSystemRegistry) : base(TeamType.Player, gameSystemRegistry)
    {
        AttrSet?.SetAttr(new AttrModifyData
        {
            Id = "base",
            IsTemp = false,
            ModifyData = new Dictionary<int, double>
            {
                { IAttrSet.ToInt(AttrType.CriticalRate), 0.05 },
                { IAttrSet.ToInt(AttrType.CriticalDamage), 0.50 }
            },
            Type = AttrModifyType.Base
        });
        DelayDeath = true;
    }

    public virtual void ActivateUltimateSkill()
    {
        EnergyMaster.Adjust(AttrSet.GetAttr(AttrType.MaxEnergy));
    }

    public override ISkill GetBasicAttack() => SkillMaster.GetSkill(BasicAttackId);
    /// <summary> 判断技能是否包含指定键的技能 </summary>
    public new bool HasSkill(string skillId) => SkillMaster.ContainsKey(skillId);
    /// <summary> 是否拥有终结技 </summary>
    public virtual bool HasUltimateSkill() => HasSkill(UltimateSkillId);
    /// <summary> 是否拥有追加攻击 </summary>
    public virtual bool HasFollowUpAttack() => false;
    /// <summary> 获取战技技能 </summary>
    public virtual ISkill GetFightSkill() => SkillMaster.GetSkill(FightSkillId);
    /// <summary> 获取终结技技能 </summary>
    public virtual ISkill GetUltimateSkill() => SkillMaster.GetSkill(UltimateSkillId);
    /// <summary> 获取追加攻击技能 </summary>
    public virtual ISkill GetFollowUpAttack() => SkillMaster.GetSkill(FollowUpAttackId);

    public override void DisplayActorInfo()
    {
        ConsoleColor healthColor = Team == TeamType.Player ? ConsoleColor.Green : ConsoleColor.Red;
        var printer = new ConsolePrinter();
        printer.Add($"{Name} ");
        printer.Add($"Lv.{Level} ", ConsoleColor.Magenta);
        printer.Add($"{Id} hp: ");
        printer.Add($"{HealthMaster?.Health:F2}", healthColor);
        printer.Add($"/{HealthMaster?.HealthMax:F2} ");
        printer.Add("Eg: ");
        printer.Add($"{EnergyMaster?.Energy} ", ConsoleColor.Cyan);
        printer.Add("Shield: ");
        printer.Add($"{ShieldMaster?.Shield:F2} ", ConsoleColor.Yellow);

        string GetAttrDumps(AttrType type)
        {
            Console.ForegroundColor = ConsoleColor.White;
            string attrValue = $"{AttrSet.GetAttr(type):F0}";
            Console.ResetColor();
            return $"{attrValue}({AttrSet.GetBaseAttr(type):F2}+{AttrSet.GetGainAttr(type):F2})";
        }

        printer.Add($"atk: {GetAttrDumps(AttrType.Attack)}, ");
        printer.Add($"def: {GetAttrDumps(AttrType.Defense)}, ");
        printer.Add($"spd: {GetAttrDumps(AttrType.Speed)}, ");
        printer.Add($"cri: {AttrSet.GetAttr(AttrType.CriticalRate):P}, crd: {AttrSet.GetAttr(AttrType.CriticalDamage):P}");

        if (!EffectMaster.Empty)
        {
            printer.Add("\n");
            foreach (IEffect effect in EffectMaster.Values)
            {
                if (effect is BaseEffect baseEffect)
                    printer.Add($"\t- {effect.Id} {baseEffect.Type.ToString()} {effect.LifeCycle}\n", ConsoleColor.DarkGray);
            }
        }
        printer.Display();
    }
}
