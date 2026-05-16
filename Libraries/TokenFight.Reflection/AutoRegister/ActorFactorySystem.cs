using TokenFight.Core.Interfaces.Factories;
using TokenFight.Core.Models.Entities.Actors;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Reflection.AutoRegister;

public class ActorFactorySystem : AutoFactorySystem<BaseActor, AutoActorAttribute>, IActorFactorySystem;
