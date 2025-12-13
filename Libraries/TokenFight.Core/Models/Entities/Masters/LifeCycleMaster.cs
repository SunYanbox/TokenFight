using System.Text;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Entities.Masters;

namespace TokenFight.Core.Models.Entities.Masters;

/// <summary> 效果的生命周期 </summary>
public class LifeCycleMaster: ILifeCycleMaster
{
    /// <summary> 效果所属角色 </summary>
    public required WeakReference<IActor> Owner { get; set; }

    /// <summary> 持续回合数 </summary>
    public int? Duration { get; set; }

    /// <summary> 剩余回合数 </summary>
    public int? CurrentDuration { get; set; }

    /// <summary> 最大叠层 </summary>
    public int? Stack { get; set; }

    /// <summary> 每次触发后叠层变化量 </summary>
    public int? DeltaStack { get; set; }

    /// <summary> 当前叠层 </summary>
    public int? CurrentStack { get; set; }

    /// <summary> 是否为标记类效果(标记不会在触发时自动减少层数) </summary>
    public bool? Mark { get; set; }

    /// <summary> 标记等级/标记信息 </summary>
    public int? CurrentMark { get; set; }

    /// <summary> 初始化持续时间型生命周期 </summary>
    public void InitDuration(int duration)
    {
        Duration = duration;
        CurrentDuration = duration;
    }

    /// <summary> 初始化叠层型生命周期 </summary>
    /// <param name="stack"></param>
    /// <param name="maxStack"></param>
    /// <param name="deltaStack"></param>
    public void InitStack(int stack, int maxStack, int deltaStack = -1)
    {
        Stack = maxStack;
        CurrentStack = stack;
        DeltaStack = deltaStack;
    }

    /// <summary> 初始化标记型生命周期 </summary>
    /// <param name="mark"></param>
    public void InitMark(int mark = 1)
    {
        Mark = true;
        CurrentMark = mark;
    }

    /// <summary> 是否需要结算剩余回合数 </summary>
    public bool HasDuration => Duration is >= 0 && CurrentDuration != null;

    /// <summary> 是否需要结算叠层数 </summary>
    public bool HasStack => Stack is >= 0 && DeltaStack != null && CurrentStack != null &&
                            Stack != CurrentStack && Stack != 0 && CurrentStack != 0;

    /// <summary> 是否需要结算标记数 </summary>
    public bool HasMark => Mark != null && (Mark ?? false) && CurrentMark is >= 0;

    /// <summary> 结算一次/一回合生命周期 </summary>
    public void SettlementCycle()
    {
        if (HasDuration)
        {
            if (CurrentDuration > 0) CurrentDuration--;
        }

        if (HasStack)
        {
            CurrentStack += DeltaStack;
            CurrentStack = Math.Max(Math.Min(CurrentStack ?? 0, Stack ?? 0), 0);
        }

        if (HasMark)
        {
            CurrentMark++;
        }
    }

    /// <summary> 是否已经失效 </summary>
    public bool IsInvalid()
    {
        if (HasDuration && CurrentDuration <= 0) return true;
        if (HasStack && CurrentStack <= 0) return true;
        return HasMark && CurrentMark <= 0;
    }

    public override string ToString()
    {
        StringBuilder stringBuilder = new();
        if (HasDuration) stringBuilder.Append($"Duration: {CurrentDuration}/{Duration}, ");
        if (HasStack) stringBuilder.Append($"Stack: {CurrentStack}/{Stack}, ");
        if (HasMark) stringBuilder.Append($"Mark: {CurrentMark}");
        return stringBuilder.ToString();
    }
}