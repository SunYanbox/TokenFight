using TokenFight.Core.Interfaces.Factories;
using TokenFight.Core.Models.Entities.Actors;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Reflection.AutoActor;

public class ActorFactorySystem: AutoFactorySystem<BaseActor, AutoActorAttribute>, IActorFactorySystem;