using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Interfaces.Attrs;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Interfaces.Entities;

/// <summary>
/// 表示一个演员实体接口，继承自基础实体接口
/// </summary>
public interface IActor: IEntity
{
    /// <summary>
    /// 演员的名称
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// 演员的等级
    /// </summary>
    int Level { get; set; }

    /// <summary>
    /// 演员所属的队伍类型
    /// </summary>
    TeamType Team { get; set; }

    /// <summary>
    /// 演员的属性集合
    /// </summary>
    IAttrSet AttrSet { get; protected set; }

    IHealthMaster HealthMaster { get; protected set; }
    IEnergyMaster EnergyMaster { get; protected set; }
    IShieldMaster ShieldMaster { get; protected set; }
    ISkillMaster SkillMaster { get; protected set; }
    IActionValueMaster ActionValueMaster { get; protected set; }
    IRelationshipMaster RelationshipMaster { get; protected set; }
    IEffectMaster EffectMaster { get; protected set; }
    IEnumTypeMaster<IdentityType> IdentityMaster { get; protected set; }

    /// <summary> 是否延迟角色死亡到所有行动结束 </summary>
    public bool DelayDeath { get; set; }
    /// <summary> 进入对局时触发 </summary>
    public void OnEnterGame();
    /// <summary> 回合开始时触发 </summary>
    public void RoundBegin();
    /// <summary> 回合结束时触发 </summary>
    public void RoundEnd();
    /// <summary> 死亡时触发, 清理所有被动技能 </summary>
    public void OnDeath();
    /// <summary> 销毁函数 </summary>
    public void Destroy();
    /// <summary> 判断成员是否存活 </summary>
    public bool IsLive() => HealthMaster.Health > 0;

    /// <summary> 判断技能是否包含指定键的技能 </summary>
    public bool HasSkill(string skillId);

    /// <summary> 用来在控制台显示成员基础信息, 适合用于战场显示 </summary>
    public void DisplayActorInfo();
}