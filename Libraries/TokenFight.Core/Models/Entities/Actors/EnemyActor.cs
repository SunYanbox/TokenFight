using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Effects;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Models.Entities.Actors;

[method: SetsRequiredMembers]
public abstract class EnemyActor(GameSystemRegistry gameSystemRegistry): BaseActor(TeamType.Enemy, gameSystemRegistry), IEnemy
{
    /// <summary> 默认普攻Id </summary>
    public string BasicAttackId => Id + nameof(SkillType.BasicAttack);

    public ISkill NextSkill() => GetBasicAttack();

    public override ISkill GetBasicAttack() => SkillMaster.GetSkill(BasicAttackId);
}