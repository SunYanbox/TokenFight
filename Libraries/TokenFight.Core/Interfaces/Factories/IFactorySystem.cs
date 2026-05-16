using TokenFight.Core.Interfaces.Bases;

namespace TokenFight.Core.Interfaces.Factories;

public interface IFactorySystem<TBase, TAttribute> : ISystem
    where TBase : class
    where TAttribute : Attribute
{
    IReadOnlyDictionary<string, TAttribute> GetAllRegisteredTypes();
    TBase CreateInstance(string id, object?[]? args);
    T CreateInstance<T>(string id, object?[]? args) where T : TBase;
    IEnumerable<Type> RegisteredTypes { get; }
    IEnumerable<string> GetAllIds();
    bool ContainsId(string id);
}
