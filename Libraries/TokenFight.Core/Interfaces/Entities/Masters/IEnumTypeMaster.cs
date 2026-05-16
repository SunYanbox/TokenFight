using TokenFight.Core.Interfaces.Bases;

namespace TokenFight.Core.Interfaces.Entities.Masters;

/// <summary>
/// 管理一类枚举类型的集合
/// </summary>
/// <typeparam name="T">必须是 struct 且为 Enum 类型</typeparam>
public interface IEnumTypeMaster<T> : IEnumSet<T>
    where T : struct, Enum
{

}