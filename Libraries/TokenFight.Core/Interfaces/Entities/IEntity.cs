namespace TokenFight.Core.Interfaces.Entities;

public interface IEntity
{
    string Id { get; init; }
    bool IsActive { get; set; }
}
