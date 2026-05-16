using TokenFight.Core.Models.Effects.Skills;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Interfaces.Factories;

public interface ISkillFactorySystem : IFactorySystem<BaseTalentSkill, AutoSkillAttribute>;