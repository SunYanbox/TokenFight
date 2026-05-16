using TokenFight.Core.Interfaces.Factories;
using TokenFight.Core.Models.Game;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Reflection.AutoRegister;

public class DungeonFactorySystem : AutoFactorySystem<BaseDungeon, AutoDungeonAttribute>, IDungeonFactorySystem;
