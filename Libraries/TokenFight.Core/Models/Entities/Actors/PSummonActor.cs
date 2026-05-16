using System.Diagnostics.CodeAnalysis;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Enums.Events;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Events;
using TokenFight.Core.Models.Effects.Skills;
using TokenFight.Core.Models.Events.Contexts;

namespace TokenFight.Core.Models.Entities.Actors;

public class PSummonActor : PlayerActor
{
    [SetsRequiredMembers]
    public PSummonActor(IActor owner, GameSystemRegistry gameSystemRegistry) : base(gameSystemRegistry)
    {
        IdentityMaster.Remove(IdentityType.Player);
        IdentityMaster.Add(IdentityType.PlayerSummon);
        RelationshipMaster.ParentActor = new WeakReference<IActor>(owner);
        owner.RelationshipMaster.ChildActors ??= new Dictionary<string, WeakReference<IActor>>();
        owner.RelationshipMaster.ChildActors.Add(Id, new WeakReference<IActor>(this));

        SkillMaster.Add(new DeathCallback(Id + "死亡回调", this, gameSystemRegistry));
    }

    private class DeathCallback : BaseTalentSkill
    {
        [SetsRequiredMembers]
        public DeathCallback(string id, PSummonActor source, GameSystemRegistry gameSystemRegistry) : base(id, source, gameSystemRegistry)
        {
            PassiveData!.Callbacks.Add(EventType.ActorDeath, source.OnParentDeath);
        }
    }

    private void OnParentDeath(IContext context)
    {
        if (context is DeathContext deathContext)
        {
            if (RelationshipMaster.ParentActor!.TryGetTarget(out IActor? actor))
            {
                if (deathContext.Actor == actor)
                {
                    OnDeath();
                }
            }
        }
    }
}
