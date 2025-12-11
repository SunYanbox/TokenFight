using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Constants;
using TokenFight.Core.Databases.Helpers;
using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Databases.Models;
using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Attrs;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Actions;
using TokenFight.Core.Models.Effects.Effects;
using TokenFight.Core.Models.Effects.Skills;
using TokenFight.Core.Models.Entities.Actors;
using TokenFight.Core.Models.Events.Contexts;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Game.Actors.Players;

[AutoActor(Id=GameIdTableConst.PlayerXiEr0, Team = TeamType.Player)]
public class PlayerXiEr0 : PlayerActor
{
    protected const string FactoryKey = GameIdTableConst.PlayerXiEr0;
    protected static DataActor? DataActor;
    protected const string BasicAttackSkillDBId = "普攻";
    protected static SkillEffectData? BasicAttackData0;
    protected const string FightSkillDBId = "战技";
    protected static SkillEffectData? FightSkillData0;
    protected const string UltimateSkillDBId = "终结技";
    protected static SkillEffectData? UltimateSkillData0;
    protected const string NaturalTalentDBId = "天赋";
    protected static SkillEffectData? NaturalTalentData0;

    [SetsRequiredMembers]
    public PlayerXiEr0(int level, GameSystemRegistry systemRegistry) : base(systemRegistry )
    {
        DataActor ??= (DataActor)systemRegistry.DatabaseServer.ActorTables[FactoryKey];
        BasicAttackData0 ??= DataActorHelper.GetSkillEffectData(DataActor!, SkillType.BasicAttack, BasicAttackSkillDBId);
        FightSkillData0 ??= DataActorHelper.GetSkillEffectData(DataActor!, SkillType.FightSkill, FightSkillDBId);
        UltimateSkillData0 ??= DataActorHelper.GetSkillEffectData(DataActor!, SkillType.UltimateSkill, UltimateSkillDBId);
        NaturalTalentData0 ??= DataActorHelper.GetSkillEffectData(DataActor!, SkillType.NaturalTalent, NaturalTalentDBId);

        Name = DataActor.Name;
        Level = level;

        ActorHelper.InitAttrSet(this, DataActor);

        double basicAttackRate = DataActorHelper.GetGrowthValue(BasicAttackData0!, "伤害倍率", Level, 0D);
        double pushRate = DataActorHelper.GetGrowthValue(BasicAttackData0!, "推条倍率", Level, 0D);
        SkillMaster.Add(new BasicBasicAttack(
            BasicAttackId,
            this,
            basicAttackRate,
            pushRate,
            systemRegistry ));
        
        SkillMaster.Add(new FightSkill(this));
        SkillMaster.Add(new UltimateSkill(this));
        SkillMaster.Add(new TalentSkill(this));
    }

    [method: SetsRequiredMembers]
    private class BasicBasicAttack(
        string id,
        IActor source,
        double basicAttackRate,
        double pushRate,
        GameSystemRegistry systemRegistry )
        : BasicBasicSkill(id, source, basicAttackRate, systemRegistry , 
            BasicAttackData0!.Name, 
            DataActorHelper.FormatSkillDesc(BasicAttackData0, [$"{basicAttackRate:P}", $"{Math.Abs(pushRate):P}"]),
            BasicAttackData0.SkillPointDelta,
            BasicAttackData0.Charge)
    {
        public override void Execute()
        {
            base.Execute();
            PushHelper.TakePushSingle(Source, Source, pushRate, this);
        }
    }

    private class FightSkill : BaseSkill
    {
        private double mainRate;
        private double subRate;

        [SetsRequiredMembers]
        public FightSkill(PlayerActor source)
        {
            Id = source.FightSkillId;
            Name = FightSkillData0!.Name;
            Source = source;
            Target = new WeakReference<IActor>(null!);
            Choice = SkillChoiceType.OnlyEnemy;
            Type = SkillType.FightSkill;
            AutoMakeSure = false;
            mainRate = DataActorHelper.GetGrowthValue(FightSkillData0, "伤害倍率0", source.Level, 0D);
            subRate = DataActorHelper.GetGrowthValue(FightSkillData0, "伤害倍率1", source.Level, 0D);
            Desc = DataActorHelper.FormatSkillDesc(FightSkillData0, [$"{mainRate:P}", $"{subRate:P}"]);
        }

        public override bool CanUse()
        {
            return GameSystemRegistry!.GlobalResourcesSystem.SkillPoint >= 1;
        }

        public override void Execute()
        {
            if (FightSkillData0!.SkillPointDelta != 0)
            {
                GameSystemRegistry!.GlobalResourcesSystem.AdjustSkillPoint(FightSkillData0.SkillPointDelta);
            }

            if (!ActorHelper.IsValidActor(Source)) return;
            
            IActor? target = ActorHelper.GetActorFromWeakRef(Target);
            if (ActorHelper.IsValidActor(target))
            {
                for (var i=0; i<3; i++)
                {
                    DamageHelper.TakeDirectDamageThree(Source, target!, mainRate / 3, subRate / 3, this, DamageHelper.DefaultCalculatorGet);
                }
            }
            
            if (Math.Abs(FightSkillData0.Charge) > double.Epsilon)
            {
                Source.EnergyMaster.Adjust(FightSkillData0.Charge);
            }
        }
    }
    
    private class UltimateSkill : BaseUltimateSkill
    {
        private double mainRate;
        private double subRate;
        private double ejectionRate;
        private int ejectionCount;

        [SetsRequiredMembers]
        public UltimateSkill(PlayerActor source)
        {
            Id = source.UltimateSkillId;
            Name = UltimateSkillData0!.Name;
            Source = source;
            Target = new WeakReference<IActor>(null!);
            Choice = SkillChoiceType.OnlyEnemy;
            Type = SkillType.UltimateSkill;
            AutoMakeSure = false;
            
            mainRate = DataActorHelper.GetGrowthValue(UltimateSkillData0, "伤害倍率0", source.Level, 0D);
            subRate = DataActorHelper.GetGrowthValue(UltimateSkillData0, "伤害倍率1", source.Level, 0D);
            ejectionCount = DataActorHelper.GetExtendProperty<int>(DataActor!, "终结技弹射次数");
            ejectionRate = DataActorHelper.GetGrowthValue(UltimateSkillData0, "伤害倍率2", source.Level, 0D);
            Desc = DataActorHelper.FormatSkillDesc(UltimateSkillData0, [$"{mainRate:P}", $"{subRate:P}", ejectionCount, $"{ejectionRate:P}"]);
        }

        public override bool CanUse()
        {
            return Source.EnergyMaster.IsEnergyFull && !IsUsing;
        }

        public override void Execute()
        {
            if (Source.EnergyMaster.ConsumeOnceEnergy())
            {
                EffectHelper.TakeEffectSingle(GetSelfEffect(Source, 2));
                
                IActor? target = ActorHelper.GetActorFromWeakRef(Target);
                if (ActorHelper.IsValidActor(target))
                {
                    for (var i = 0; i < 3; i++)
                    {
                        DamageHelper.TakeDirectDamageThree(Source, target!, mainRate / 3, subRate / 3, this, DamageHelper.DefaultCalculatorGet);
                    }
                }

                DamageHelper.TakeDirectDamageEjection(Source, TeamType.Enemy, ejectionRate, this, DamageHelper.DefaultCalculatorGet, ejectionCount);
                
                IsUsing = false;
            }
        }
    }

    private class TalentSkill : BaseTalentSkill
    {
        private bool _triggered = false;
        private readonly HashSet<string> _triggeredT2 = new();

        private double _extraCharge = DataActorHelper.GetExtendProperty<double>(DataActor!, "击败敌人额外回能");
        private int _markCount = DataActorHelper.GetExtendProperty<int>(DataActor!, "命中标记立即行动的层数");
        
        [SetsRequiredMembers]
        public TalentSkill(PlayerActor source): base(source.NaturalTalentId, source, GameSystemRegistry!)
        {
            Name = NaturalTalentData0!.Name;
            Desc = DataActorHelper.FormatSkillDesc(NaturalTalentData0, [_extraCharge, _markCount]);
            
            PassiveData!.Callbacks.Add(EventType.Damage, HandleTalentDamage);
            PassiveData.Callbacks.Add(EventType.ActionEnd, HandleTalent_重置CD);
        }
        
        private void HandleTalentDamage(IContext data)
        {
            if (data is not DamageContext context) return;
            if (context.Source is not PlayerXiEr0 xier) return;
            IActor target = context.Target;
            string markId = Source.Id + "希尔标记";
            string debuffId = Source.Id + "希尔针对";
            // 命中标记
            // 层数大于指定层数后使得自身立即行动
            if (_triggeredT2.Add(target.Id))
            {
                EffectHelper.TakeEffectSingle(GetMarkEffect(Source, target));
                
                
                if (target.EffectMaster.Has(markId))
                {
                    IEffect mark = target.EffectMaster.Get(markId);
                    if (mark.LifeCycle.CurrentMark >= _markCount)
                    {
                        mark.LifeCycle.CurrentMark -= _markCount;
                        if (mark.LifeCycle.CurrentMark == 0)
                        {
                            target.EffectMaster.Remove(markId);
                        }
                        EffectHelper.TakeEffectSingle(GetDebuffEffect(Source, target));
                        PushHelper.TakeSingleActionNow(Source, Source, this);
                    }
                }
            }
            if (target.IsLive() || !context.IsKill) return;
            // 击杀再现
            ActionUnit? actionUnit = GameSystemRegistry!.ActionManagerSystem.NewestAction;
            if (actionUnit == null 
                || actionUnit.IsExtraTurn && actionUnit.OwnActor == Source 
                || _triggered) return;
            GameSystemRegistry!.GlobalResourcesSystem.AdjustSkillPoint(1);
            CreateActionHelper.CreateNewExtraTurn(xier);
            // 击杀自拐
            EffectHelper.TakeEffectSingle(GetSelfEffect(Source, 1));
            // 击杀额外充能
            Source.EnergyMaster.Adjust(_extraCharge);
            // 击杀随机转移标记和负面
            if (target.EffectMaster.Has(markId))
            {
                IEffect mark = target.EffectMaster.Get(markId);
                IActor[] enemies = ActorHelper.GetActorsByTeamWithLifeAndValid(TeamType.Enemy);
                if (enemies.Length > 0)
                {
                    for (int i = 0; i < mark.LifeCycle.CurrentMark; i++)
                    {
                        IActor? t = ActorHelper.GetRandomActor(enemies);
                        t?.EffectMaster.Apply(GetMarkEffect(Source, t));
                    }
                }
            }
            if (target.EffectMaster.Has(debuffId))
            {
                IEffect debuff = target.EffectMaster.Get(debuffId);
                IActor[] enemies = ActorHelper.GetActorsByTeamWithLifeAndValid(TeamType.Enemy);
                if (enemies.Length > 0)
                {
                    for (int i = 0; i < debuff.LifeCycle.CurrentStack; i++)
                    {
                        IActor? t = ActorHelper.GetRandomActor(enemies);
                        t?.EffectMaster.Apply(GetDebuffEffect(Source, t));
                    }
                }
            }
            _triggered = true;
        }
        
        private void HandleTalent_重置CD(IContext data)
        {
            if (data is not ActionContext) return;
            _triggered = false;
            _triggeredT2.Clear();
        }
    }
    
    // 获取标记buff
    private static BaseEffect GetMarkEffect(IActor source, IActor target)
    {
        return EffectCreateHelper.CreateMark(source.Id + "希尔标记", source, target);
    }
    
    // 获取负面buff
    private static BaseEffect GetDebuffEffect(IActor source, IActor target)
    {
        BaseEffectPctGain effect = new BaseEffectPctGain(source, target, source.Id + "希尔针对",
            new Dictionary<int, double>
            {
                { IAttrSet.ToInt(AttrType.Vulnerability), 0.1 }
            },
            GameSystemRegistry!)
        {
            Type = EffectType.Debuff
        };
        effect.InitStack(1, 999, 0);
        return effect;
    }
    
    // 获取自拐buff
    private static BaseEffect GetSelfEffect(IActor source, int initial = 1)
    {
        return EffectCreateHelper.CreatePctGain(source.Id + "希尔自拐", source, source,
            new Dictionary<int, double>
            {
                { IAttrSet.ToInt(AttrType.Attack), 0.15 },
                { IAttrSet.ToInt(AttrType.Speed), 0.15 },
                { IAttrSet.ToInt(AttrType.DamageIncrease), 0.20 },
            }, duration: 2, initStack: initial, maxStack: 2);
    }
    
    
}