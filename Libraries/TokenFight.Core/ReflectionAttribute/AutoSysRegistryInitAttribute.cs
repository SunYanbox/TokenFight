namespace TokenFight.Core.ReflectionAttribute;

/// <summary>
/// 自动初始化游戏系统注册表的标记
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
    AllowMultiple = false,
    Inherited = false)]
public class AutoSysRegistryInitAttribute : Attribute;