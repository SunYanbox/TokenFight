namespace TokenFight.Core.ReflectionAttribute;

/// <summary>
/// 自动生成副本
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class AutoDungeonAttribute: AutoBaseAttribute
{
    public bool IsActive { get; set; } = true;
}