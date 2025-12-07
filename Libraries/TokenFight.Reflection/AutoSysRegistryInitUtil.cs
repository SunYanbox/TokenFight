using System.Reflection;
using TokenFight.Core.Models;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Reflection;

/// <summary>
/// 自动初始化带AutoSysRegistryInitAttribute标记的类属性
/// </summary>
public static class AutoSysRegistryInitUtil
{
    private static bool _initialized = false;
    private static readonly Lock Lock = new();
    
    public static void Initialize(GameSystemRegistry systemRegistry)
    {
        if (_initialized) return;
        
        lock (Lock)
        {
            if (_initialized) return;
            
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    // 获取所有标记了 AutoSysRegistryInitAttribute 的类
                    IEnumerable<Type> types = assembly.GetTypes()
                        .Where(t => t.GetCustomAttribute<AutoSysRegistryInitAttribute>() != null);
                    
                    foreach (Type type in types)
                    {
                        InitializeClass(type, systemRegistry);
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    Console.WriteLine($"[WARN] 无法加载程序集：{assembly.FullName}");
                    continue;
                }
            }
            
            _initialized = true;
        }
    }
    
    /// <summary>
    /// 初始化单个类
    /// </summary>
    private static void InitializeClass(Type type, GameSystemRegistry registry)
    {
        try
        {
            var property = type.GetProperty("GameSystemRegistry", 
                BindingFlags.Public | BindingFlags.Static);
            
            // 确认属性存在且类型匹配
            if (property == null || 
                property.PropertyType != typeof(GameSystemRegistry) ||
                !property.CanWrite)
            {
                throw new InvalidOperationException(
                    $"类型 {type.FullName} 必须具有 'public static GameSystemRegistry? GameSystemRegistry {{ protected get; set; }}' 属性");
            }
            
            // 获取受保护的 setter 并调用
            var setMethod = property.GetSetMethod(true);
            if (setMethod == null)
            {
                throw new InvalidOperationException(
                    $"类型 {type.FullName} 的属性 'GameSystemRegistry' 必须具有 setter 方法");
            }
            
            setMethod.Invoke(null, [registry]);
            
            Console.WriteLine($"已为 {type.FullName} 初始化 GameSystemRegistry");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"初始化 {type.FullName} 失败: {ex.Message}");
            throw;
        }
    }
}