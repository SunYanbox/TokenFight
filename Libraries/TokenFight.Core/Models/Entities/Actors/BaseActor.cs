using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Consoles.Display;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Attrs;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;
using TokenFight.Core.Models.Attrs;
using TokenFight.Core.Models.Effects.Effects;
using TokenFight.Core.Models.Entities.Masters;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Models.Entities.Actors;

[AutoSysRegistryInit]
public abstract class BaseActor: IActor
{
    public static GameSystemRegistry? GameSystemRegistry { protected get; set; }

    /// <summary> 成员销毁 </summary>
    public virtual void Destroy()
    {
        GameSystemRegistry?.IdGenerateSystem.ReleaseId(Id);
    }

    [SetsRequiredMembers]
    protected BaseActor(TeamType team, GameSystemRegistry gameSystemRegistry)
    {
        GameSystemRegistry ??= gameSystemRegistry;
        Id = GameSystemRegistry.IdGenerateSystem.GetNewId(team);
        Name = "佚名";
        Level = 1;
        Team = team;
        IdentityMaster = new EnumTypeMaster<IdentityType>();
        switch (Team)
        {
            case TeamType.Player:
                IdentityMaster.Add(IdentityType.Player);
                break;
            case TeamType.Enemy:
                IdentityMaster.Add(IdentityType.EnemyCommon);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(Team), "未知的队伍类型");
        }
        IsActive = true;
        AttrSet = new AttrSet();
        AttrSet.SetAttr(new AttrModifyData
        {
            IsTemp = false,
            ModifyData = new Dictionary<int, double> { { IAttrSet.ToInt(AttrType.Health), 1 } },
            Type = AttrModifyType.Base,
            Id = "base"
        });
        HealthMaster = new HealthMaster(this);
        EnergyMaster = new EnergyMaster(this);
        ActionValueMaster = new ActionValueMaster(this);
        SkillMaster = new SkillMaster
        {
            Owner = new WeakReference<IActor>(this)
        };
        EffectMaster = new EffectMaster(this);
        RelationshipMaster = new RelationshipMaster(this);
        ShieldMaster = new ShieldMaster(this);
        DelayDeath = false;
        _hadTriggerDeathContext = false;
    }
    public abstract ISkill GetBasicAttack();

    public required string Id { get; init; }
    public bool IsActive { get; set; }
    public required string Name { get; set; }
    public int Level { get; set; }
    public TeamType Team { get; set; }
    public required IAttrSet AttrSet { get; set; }
    public required IHealthMaster HealthMaster { get; set; }
    public required IEnergyMaster EnergyMaster { get; set; }
    public required IShieldMaster ShieldMaster { get; set; }
    public required ISkillMaster SkillMaster { get; set; }
    public required IEffectMaster EffectMaster { get; set; }
    public IEnumTypeMaster<IdentityType> IdentityMaster { get; set; }
    public required IActionValueMaster ActionValueMaster { get; set; }
    public required IRelationshipMaster RelationshipMaster { get; set; }
    public bool DelayDeath { get; set; }
    private bool _hadTriggerDeathContext;

    public virtual void OnEnterGame()
    {

    }

    public virtual void RoundBegin()
    {
        EffectMaster.RoundBegin();
    }

    public virtual void RoundEnd()
    {
        EffectMaster.RoundEnd();
    }

    public virtual void OnDeath()
    {
        if (_hadTriggerDeathContext) return;
        SkillMaster.UnsubscriptAll();
        EventHelper.TriggerDeathContext(this);
        _hadTriggerDeathContext = true;
    }

    public bool HasSkill(string skillId) => SkillMaster.HasSkill(skillId);

    public virtual void DisplayActorInfo()
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
            string attrValue = $"{AttrSet.GetAttr(type):F2}";
            Console.ResetColor();
            return $"{attrValue}({AttrSet.GetBaseAttr(type):F2}+{AttrSet.GetGainAttr(type):F2})";
        }

        printer.Add($"atk: {GetAttrDumps(AttrType.Attack)}, ");
        printer.Add($"def: {GetAttrDumps(AttrType.Defense)}, ");
        printer.Add($"spd: {GetAttrDumps(AttrType.Speed)}");

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