namespace TokenFight.Core.Interfaces.Effects;

/// <summary> 终结技接口 </summary>
public interface IUltimateSkill: ISkill
{
    /// <summary> 是否已经在终结技轮询中被启用 </summary>
    public bool IsUsing { get; set; }

    public new bool CanUse()
    {
        return Source.EnergyMaster.IsEnergyFull;
    }
}