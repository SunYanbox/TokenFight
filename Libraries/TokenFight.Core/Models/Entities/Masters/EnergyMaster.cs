using TokenFight.Core.Enums.Attrs;
using TokenFight.Core.Interfaces.Attrs;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Entities.Masters;

public class EnergyMaster: IEnergyMaster
{
    public WeakReference<IActor> Owner { get; set; }
    private double MaxEnergy { get; set; }
    private double CurrentEnergy { get; set; }
    private IAttrSet? AttrSet { get; set; }

    public EnergyMaster(IActor owner)
    {
        Owner = new WeakReference<IActor>(owner);
        AttrSet = owner.AttrSet;
        MaxEnergy = (AttrSet?.GetBaseAttr(AttrType.MaxEnergy) ?? 0)
                    + (AttrSet?.GetGainAttr(AttrType.MaxEnergy) ?? 0);
        MaxEnergy = Math.Max(MaxEnergy, 1);
        CurrentEnergy = MaxEnergy * 0.5;
    }
    public double Energy
    {
        get
        {
            UpdateMaxEnergy();
            CurrentEnergy = Math.Max(Math.Min(CurrentEnergy, MaxEnergy), 0);
            return CurrentEnergy;
        }
    }
    public double EnergyMax
    {
        get
        {
            UpdateMaxEnergy();
            return MaxEnergy;
        }
    }

    private void UpdateMaxEnergy()
    {
        double maxEnergy = (AttrSet?.GetBaseAttr(AttrType.MaxEnergy) ?? 0)
                           + (AttrSet?.GetGainAttr(AttrType.MaxEnergy) ?? 0);
        // 最大能量值变化
        if (Math.Abs(maxEnergy - MaxEnergy) >= double.Epsilon)
        {
            double change = maxEnergy / Math.Max(MaxEnergy, 1);
            CurrentEnergy *= change;
            MaxEnergy = maxEnergy;
        }
    }
    public void Adjust(double delta)
    {
        UpdateMaxEnergy();
        CurrentEnergy += delta;
        CurrentEnergy = Math.Max(Math.Min(CurrentEnergy, MaxEnergy), 0);
    }
    public bool IsEnergyFull => CurrentEnergy >= MaxEnergy;
    public bool ConsumeOnceEnergy(double rate = 1.0)
    {
        if (IsEnergyFull)
        {
            CurrentEnergy -= MaxEnergy * rate;
            if (CurrentEnergy < 0) CurrentEnergy = 0;
            return true;
        }
        return false;
    }
}