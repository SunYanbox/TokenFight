namespace TokenFight.Core.Interfaces.Entities.Masters;

/// <summary>
/// 行动值系统
///
/// 绑定一个AttributeSet用于跟踪速度变化
/// </summary>
public interface IActionValueMaster : IMaster
{
    /// <summary> 获取当前行动值 </summary>
    public double ActionValue { get; }

    /// <summary> 根据速度重置行动值 </summary>
    public void Reset();

    /// <summary> 行动指定数值的行动值 </summary>
    public void Action(double actionValue);

    /// <summary> 按照percent拉条或推条, percent>0时推条, percent小于0时拉条 </summary>
    public void Push(double percent);
}