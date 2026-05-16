namespace TokenFight.Core.ReflectionAttribute;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class AutoBaseAttribute : Attribute
{
    public required string Id { get; set; }
}