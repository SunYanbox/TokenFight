using TokenFight.Core.Enums.Entities;
using TokenFight.Core.Interfaces.Entities;
using TokenFight.Core.Interfaces.Systems.Combatant;

namespace TokenFight.Core.Models.Systems.Combatant;

public class ActorPositionSystem: IActorPositionSystem
{
    private readonly List<string> _players = [];
    private readonly List<string> _enemies = [];

    public void Init() { }

    public void Reset()
    {
        _players.Clear();
        _enemies.Clear();
    }

    public int PlayerCount => _players.Count;
    public int EnemyCount => _enemies.Count;
    public List<string> PlayerPosition => _players.AsReadOnly().ToList();
    public List<string> EnemyPosition => _enemies.AsReadOnly().ToList();
    public void Append(string id, TeamType team)
    {
        switch (team)
        {
            case TeamType.Player:
                _players.Add(id);
                break;
            case TeamType.Enemy:
                _enemies.Add(id);
                break;
        }
    }

    public void Insert(int position, string id, TeamType team)
    {
        switch (team)
        {
            case TeamType.Player:
                _players.Insert(position, id);
                break;
            case TeamType.Enemy:
                _enemies.Insert(position, id);
                break;
        }
    }

    public int GetPosition(string id, TeamType team)
    {
        switch (team)
        {
            case TeamType.Player:
                return _players.IndexOf(id);
            case TeamType.Enemy:
                return _enemies.IndexOf(id);
        }
        return -1;
    }

    public void Remove(string id, TeamType team)
    {
        switch (team)
        {
            case TeamType.Player:
                _players.Remove(id);
                break;
            case TeamType.Enemy:
                _enemies.Remove(id);
                break;
        }
    }

    public void UpdateActorRelationship(IActorManagerSystem actorManagerSystem)
    {
        Dictionary<string, IActor> players = actorManagerSystem.AllActors
            .Where(x => x.Value.Team == TeamType.Player)
            .ToDictionary();
        Dictionary<string, IActor> enemies = actorManagerSystem.AllActors
            .Where(x => x.Value.Team == TeamType.Enemy)
            .ToDictionary();

        List<string> playerPositions = PlayerPosition;
        List<string> enemyPosition = EnemyPosition;

        SetupActorRelationships(players, playerPositions);
        SetupActorRelationships(enemies, enemyPosition);
    }

    private void SetupActorRelationships(Dictionary<string, IActor> actors, List<string> positions)
    {
        foreach (KeyValuePair<string, IActor> actorEntry in actors)
        {
            IActor actor = actorEntry.Value;
            actor.RelationshipMaster.LeftActor = null;
            actor.RelationshipMaster.RightActor = null;

            int position = positions.IndexOf(actorEntry.Key);

            if (position > 0)
            {
                actor.RelationshipMaster.LeftActor = new WeakReference<IActor>(actors[positions[position - 1]]);
            }

            if (position < positions.Count - 1)
            {
                actor.RelationshipMaster.RightActor = new WeakReference<IActor>(actors[positions[position + 1]]);
            }
        }
    }
}