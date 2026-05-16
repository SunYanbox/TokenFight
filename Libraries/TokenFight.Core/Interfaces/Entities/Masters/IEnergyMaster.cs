namespace TokenFight.Core.Interfaces.Entities.Masters;

public interface IEnergyMaster : IMaster
{
    /// <summary> 获取当前能量值 </summary>
    public double Energy { get; }
    /// <summary> 获取最大能量值 </summary>
    public double EnergyMax { get; }
    /// <summary> 调整能量 </summary>
    public void Adjust(double delta);
    /// <summary> 能量是否充满 </summary>
    public bool IsEnergyFull { get; }
    /// <summary> 消耗一次等于最大能量rate比率的能量 </summary>
    public bool ConsumeOnceEnergy(double rate = 1.0);
}