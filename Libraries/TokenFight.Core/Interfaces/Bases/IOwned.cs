using TokenFight.Core.Interfaces.Entities;

namespace TokenFight.Core.Interfaces.Bases;

/// <summary> 有拥有者的 </summary>
public interface IOwned
{
    WeakReference<IActor> Owner { get; set; }
}