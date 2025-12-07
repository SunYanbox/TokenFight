using TokenFight.Core.Databases.Models;
using TokenFight.Core.Databases.Models.Dungeons;
using TokenFight.Core.Databases.Models.Profiles;
using TokenFight.Core.Interfaces.Bases;

namespace TokenFight.Core.Databases.Interfaces;

public interface IDatabaseServer : ISystem
{
    Dictionary<string, DataActor> ActorTables { get; init; }
    Dictionary<string, Profile> ProfileTables { get; init; }
    public Dictionary<string, DungeonInfo> DungeonInfoTables { get; init; }
}