using System.Reflection;
using TokenFight.Core.Consoles.Display;
using TokenFight.Core.Interfaces.Factories;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Reflection;

/// <summary>
/// 泛型 | 自动工厂系统
/// </summary>
/// <typeparam name="TBase"></typeparam>
/// <typeparam name="TAttribute"></typeparam>
public class AutoFactorySystem<TBase, TAttribute> : IFactorySystem<TBase, TAttribute>
    where TBase : class
    where TAttribute : AutoBaseAttribute
{
    private readonly Dictionary<string, Type> _registeredTypes = new();
    private readonly Dictionary<string, TAttribute> _typeAttributes = new();
    private bool _isInitialized = false;

    public void Reset()
    {
        Console.WriteLine($"[Error] {GetType().Name}系统逻辑上不应该会调用Reset()方法");
        throw new Exception($"{GetType().Name}系统逻辑上不应该会调用Reset()方法");
    }

    public void Init()
    {
        if (_isInitialized) return;

        // 获取当前程序集中所有类型
        // var assembly = Assembly.GetExecutingAssembly();
        Assembly[] assembly = AppDomain.CurrentDomain.GetAssemblies();
        Type[] types = assembly.SelectMany(x => x.GetTypes()).ToArray();

        foreach (Type type in types)
        {
            // 检查是否具有 TAttribute 特性并且是 BaseActor 的派生类
            var attribute = type.GetCustomAttribute<TAttribute>();
            if (attribute == null || !typeof(TBase).IsAssignableFrom(type)) continue;
            string id = attribute.Id;

            if (_registeredTypes.TryAdd(id, type))
            {
                Console.WriteLine($"[自动注册反射] 注册{typeof(TBase).Name}: {id}");
                _typeAttributes[id] = attribute;
            }
            else
            {
                // 处理重复ID的情况
                throw new InvalidOperationException($"[角色反射] 重复的注册ID: {id}");
            }
        }

        _isInitialized = true;
    }

    // 获取所有注册的类型信息
    public IReadOnlyDictionary<string, TAttribute> GetAllRegisteredTypes() => _typeAttributes;

    // 根据ID创建实例
    public TBase CreateInstance(string id, object?[]? args)
    {
        try
        {
            if (_registeredTypes.TryGetValue(id, out Type? type))
            {
                return (TBase)Activator.CreateInstance(type, args)!;
            }
            throw new KeyNotFoundException($"未找到ID为 '{id}' 的注册类型");
        }
        catch (Exception e)
        {
            ConsolePrinter consolePrinter = new();
            consolePrinter.Add($"[{GetType().Name}] 创建实例时出错: {e.Message}\n\t{e.StackTrace}" +
                               $"\n\t创建实例的ID: {id}\n\t创建实例的参数: {args} (Count={args?.Length ?? 0})\n"
                , ConsoleColor.Red);
            if (args != null)
            {
                consolePrinter.Add("参数列表: [ ", ConsoleColor.Blue);
                foreach (object? arg in args)
                {
                    if (arg != null)
                    {
                        if (arg is IEnumerable<object>)
                        {
                            consolePrinter.Add($"[ {string.Join(", ", arg)} ]", ConsoleColor.Blue);
                        }
                        else if (arg is IDictionary<object, object> dict)
                        {
                            consolePrinter.Add($"{{ {string.Join(", ", dict.Select(x => $"{x.Key}: {x.Value}"))} }}",
                                ConsoleColor.Blue);
                        }
                        else
                        {
                            consolePrinter.Add(arg.ToString() ?? "null", ConsoleColor.Blue);
                        }
                    }
                    else
                    {
                        consolePrinter.Add("null", ConsoleColor.Gray);
                    }
                    consolePrinter.Add(" | ", ConsoleColor.Red);
                }
                consolePrinter.Add(" ]\n", ConsoleColor.Blue);
            }
            consolePrinter.Display();
            throw;
        }
    }

    // 根据ID创建实例（泛型版本）
    public T CreateInstance<T>(string id, object?[]? args) where T : TBase
    {
        TBase instance = CreateInstance(id, args);
        return (T)instance;
    }

    public IEnumerable<Type> RegisteredTypes => _registeredTypes.Values;

    // 获取所有可用的ID
    public IEnumerable<string> GetAllIds() => _registeredTypes.Keys;

    // 检查ID是否存在
    public bool ContainsId(string id) => _registeredTypes.ContainsKey(id);
}