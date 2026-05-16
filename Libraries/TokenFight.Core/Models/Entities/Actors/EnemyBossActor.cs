using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Entities;

namespace TokenFight.Core.Models.Entities.Actors;

public class EnemyBossActor : EnemyActor
{
    [SetsRequiredMembers]
    public EnemyBossActor(GameSystemRegistry gameSystemRegistry) : base(gameSystemRegistry)
    {
        IdentityMaster.Remove(IdentityType.EnemyCommon);
        IdentityMaster.Add(IdentityType.EnemyBoss);
    }
}
