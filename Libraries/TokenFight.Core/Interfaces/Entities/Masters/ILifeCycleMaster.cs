namespace TokenFight.Core.Interfaces.Entities.Masters;

public interface ILifeCycleMaster : IMaster
{
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
    public void InitDuration(int duration);

    /// <summary> 初始化叠层型生命周期 </summary>
    /// <param name="stack"></param>
    /// <param name="maxStack"></param>
    /// <param name="deltaStack"></param>
    public void InitStack(int stack, int maxStack, int deltaStack = -1);

    /// <summary> 初始化标记型生命周期 </summary>
    /// <param name="mark"></param>
    public void InitMark(int mark = 1);

    /// <summary> 是否需要结算剩余回合数 </summary>
    public bool HasDuration { get; }

    /// <summary> 是否需要结算叠层数 </summary>
    public bool HasStack { get; }

    /// <summary> 是否需要结算标记数 </summary>
    public bool HasMark { get; }

    /// <summary> 结算一次/一回合生命周期 </summary>
    public void SettlementCycle();

    /// <summary> 是否已经失效 </summary>
    public bool IsInvalid();
}