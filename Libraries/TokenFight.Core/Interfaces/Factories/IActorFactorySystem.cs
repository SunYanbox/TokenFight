using TokenFight.Core.Models.Entities.Actors;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Interfaces.Factories;

public interface IActorFactorySystem : IFactorySystem<BaseActor, AutoActorAttribute>;