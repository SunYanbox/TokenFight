using TokenFight.Core.Consoles;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Interfaces.Controls;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Models.Actions;
using TokenFight.Core.Models.Entities.Actors;

namespace TokenFight.Core.Models.Controls;

/// <summary> 代理敌人行动的简单逻辑 </summary>
public class AgentEnemy : IAgent
{
    public Random Random { get; init; } = new();


    public bool CanHandle(IActor actor) => actor.Team == TeamType.Enemy && actor is EnemyActor;


    public bool HandleSkillChoice(IActor actor, ActionUnit actionUnit)
    {
        if (!CanHandle(actor)) return false;
        if (actor is not EnemyActor enemyActor) return false;

        IAgent.OutputTalentData(enemyActor);

        ISkill skill = enemyActor.NextSkill();
        actionUnit.Skill = skill;
        Console.WriteLine($"[敌人技能] {skill.Name} {skill.Desc}");
        return false;
    }


    public void HandleTargetChoice(IActor actor, ActionUnit actionUnit)
    {
        if (!CanHandle(actor)) return;
        if (actionUnit.Skill == null) return;
        ISkill skill = actionUnit.Skill;
        IActor[] canChoice = SelectionUtil.GetActorWhereSkillChoice(skill);
        if (canChoice.Length == 0) return;
        skill.Target = new WeakReference<IActor>(canChoice[Random.Next(0, canChoice.Length)]);
    }
}
