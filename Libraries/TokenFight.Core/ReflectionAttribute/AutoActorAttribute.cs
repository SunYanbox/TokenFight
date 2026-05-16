using TokenFight.Core.Enums.Entities;

namespace TokenFight.Core.ReflectionAttribute;

/// <summary>
/// 自动注册的Actor
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class AutoActorAttribute : AutoBaseAttribute
{
    public TeamType Team { get; set; }
    public string? Name { get; set; }
    public bool IsActive { get; set; } = true;
}
