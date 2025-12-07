using TokenFight.Core.Interfaces.Effects;

namespace TokenFight.Core.Interfaces.Entities;

public interface IEnemy
{
    ISkill NextSkill();
}