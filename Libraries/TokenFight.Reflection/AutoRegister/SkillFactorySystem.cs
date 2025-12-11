using TokenFight.Core.Interfaces.Factories;
using TokenFight.Core.Models.Effects.Skills;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Reflection.AutoRegister;

public class SkillFactorySystem: AutoFactorySystem<BaseTalentSkill, AutoSkillAttribute>, ISkillFactorySystem;