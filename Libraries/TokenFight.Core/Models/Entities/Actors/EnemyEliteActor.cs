using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Entities;

namespace TokenFight.Core.Models.Entities.Actors;

/// <summary> 精英怪基类 </summary>
public abstract class EnemyEliteActor: EnemyActor
{
    [SetsRequiredMembers]
    protected EnemyEliteActor(GameSystemRegistry gameSystemRegistry): base(gameSystemRegistry)
    {
        IdentityMaster.Remove(IdentityType.EnemyCommon);
        IdentityMaster.Add(IdentityType.EnemyElite);
    }
}