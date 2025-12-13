using TokenFight.Core.Interfaces.Bases;

namespace TokenFight.Core.Interfaces.Entities.Masters;

public interface IMaster: IOwned
{
    protected void SubscribeEvents() { }
    protected void UnsubscribeEvents() { }
}