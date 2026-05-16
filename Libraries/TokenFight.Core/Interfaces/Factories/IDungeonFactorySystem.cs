using TokenFight.Core.Models.Game;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Interfaces.Factories;

public interface IDungeonFactorySystem : IFactorySystem<BaseDungeon, AutoDungeonAttribute>;