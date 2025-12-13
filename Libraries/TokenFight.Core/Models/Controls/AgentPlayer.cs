using TokenFight.Core.Consoles;
using TokenFight.Core.Enums.Actions;
using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Helpers;
using TokenFight.Core.Interfaces.Controls;
using TokenFight.Core.Interfaces.Effects;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.FStream;
using TokenFight.Core.Interfaces.Systems.Combatant;
using TokenFight.Core.Models.Actions;
using TokenFight.Core.Models.Entities.Actors;

namespace TokenFight.Core.Models.Controls;

/// <summary> 代理玩家角色行动, 在控制台显示技能选择信息的接口 </summary>
public class AgentPlayer(
    ILocalLog localLog,
    IActionManagerSystem actionManagerSystem,
    IActorManagerSystem actorManagerSystem,
    IActorPositionSystem actorPositionSystem): IAgent
{
    public Random Random { get; init; } = new();

    public bool CanHandle(IActor actor) => actor.Team == TeamType.Player && actor is PlayerActor;


    public bool HandleSkillChoice(IActor actor, ActionUnit actionUnit)
    {
        if (!CanHandle(actor)) return false;
        if (actor is not PlayerActor playerActor) return false;
        ISkill? skill = null;
        if (actionUnit.Skill == null)
        {
            IAgent.OutputTalentData(playerActor);

            List<ISkill> canChoiceSkill = [];
            canChoiceSkill.AddRange(playerActor.SkillMaster.GetActiveSkillsApartFromUltimate());
            canChoiceSkill.AddRange(ActorHelper.GetAllUltimateSkillCanUse());
            try
            {
                skill = SelectionUtil.SelectFromList(
                    canChoiceSkill.ToArray(),
                    x => $"{x.Name} {x.Desc}",
                    "请选择你的技能: ",
                    quitValue: null
                );
            }
            catch (Exception e)
            {
                localLog.LogError($"选择{actor}的技能时出错: {e.Message}\n\t{e.StackTrace}");
                return false;
            }
            if (skill is IUltimateSkill ultimateSkill)
            {
                ultimateSkill.IsUsing = true;
                actionManagerSystem.CreateAction(ActionPriority.Ultimate, skill: skill);
                // 释放终结技后退出
                return true;
            }
            actionUnit.Skill = skill;
        }

        return false;
    }


    public void HandleTargetChoice(IActor actor, ActionUnit actionUnit)
    {
        if (!CanHandle(actor)) return;
        if (actionUnit.Skill == null) return;
        ISkill skill = actionUnit.Skill;

        IActor[] canChoiceActor = SelectionUtil.GetActorWhereSkillChoice(skill);
        actorPositionSystem.UpdateActorRelationship(actorManagerSystem);
        if (canChoiceActor.Length == 0) return;

        if (canChoiceActor.Length == 1)
        {
            skill.Target = new WeakReference<IActor>(canChoiceActor[0]);
            return;
        }

        skill.Target = new WeakReference<IActor>(SelectionUtil.SelectFromList(
            ActorHelper.SortActorsByPosition(canChoiceActor),
            x => $"{x.Name}({x.Id}) {x.HealthMaster?.Health:F2}/{x.HealthMaster?.HealthMax:F2}",
            "请选择你的技能目标: ",
            quitValue: null)!);
    }
}