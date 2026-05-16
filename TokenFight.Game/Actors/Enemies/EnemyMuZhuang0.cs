using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Constants;
using TokenFight.Core.Databases.Helpers;
using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Databases.Models.Growth;
using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models;
using TokenFight.Core.Models.Effects;
using TokenFight.Core.Models.Effects.Skills;
using TokenFight.Core.Models.Entities.Actors;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Game.Actors.Enemies;

[AutoActor(Id = GameIdTableConst.EnemyMuZhuang0, Team = TeamType.Enemy)]
public class EnemyMuZhuang0 : EnemyActor
{
    protected const string FactoryKey = GameIdTableConst.EnemyMuZhuang0;
    protected static DataActor? DataActor;
    protected const string BasicAttackSkillId = "自疗";
    protected static SkillEffectData? BasicAttackData0;

    [SetsRequiredMembers]
    public EnemyMuZhuang0(int level, GameSystemRegistry systemRegistry)
        : base(systemRegistry)
    {
        DataActor ??= (DataActor)systemRegistry.DatabaseServer.ActorTables[FactoryKey];
        BasicAttackData0 ??= DataActorHelper.GetSkillEffectData(DataActor!, SkillType.BasicAttack, BasicAttackSkillId);

        Name = DataActor.Name;
        Level = level;

        ActorHelper.InitAttrSet(this, DataActor);

        double healRate = DataActorHelper.GetGrowthValue(BasicAttackData0!, "治疗倍率", Level, 0D);
        SkillMaster!.Add(new HealSelf(BasicAttackId, BasicAttackData0!.Name,
            DataActorHelper.FormatSkillDesc(BasicAttackData0, [$"{healRate:P}"]),
            healRate,
            this,
            new WeakReference<IActor>(this),
            new PassiveData(systemRegistry.EventSystem, systemRegistry.LocalLog)));
    }

    private class HealSelf : BaseSkill
    {
        private double? _rate;

        [SetsRequiredMembers]
        public HealSelf(string id, string name, string desc, double rate, IActor source,
            WeakReference<IActor> target, PassiveData passiveData)
        {
            Id = id;
            Name = name;
            Desc = desc;
            _rate = rate;
            Source = source;
            Target = target;
            Choice = SkillChoiceType.OnlySelf;
            PassiveData = passiveData;
            AutoMakeSure = false;
            Type = SkillType.BasicAttack;
        }

        public override bool CanUse() => true;

        public override void Execute()
        {
            if (ActorHelper.IsValidActor(Source))
                HealHelper.TakeHealSingle(Source, Source, _rate ?? 0, this);
        }
    }
}
