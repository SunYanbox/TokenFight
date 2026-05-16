namespace TokenFight.Core.Enums.Effects;

/// <summary> 技能可选择的对象类型 </summary>
public enum SkillChoiceType
{
    /// <summary> 只能对自己释放 </summary>
    OnlySelf,
    /// <summary> 只能对自己的队友释放 </summary>
    OnlyAllies,
    /// <summary> 对我方任何人释放 </summary>
    AnyAllies,
    /// <summary> 只能对敌人释放 </summary>
    OnlyEnemy
}
