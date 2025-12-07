using System.Text.Json;
using TokenFight.Core.Constants;
using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Databases.Models;
using TokenFight.Core.Databases.Models.Profiles;

namespace TokenFight.Core.Databases;


public class DatabaseServer: IDatabaseServer
{
    public DatabaseServer()
    {
        var dataFolder = GameConst.DataFolder;
        var actorFolder = Path.Combine(dataFolder, "actors");
        var profilesFolder = Path.Combine(dataFolder, "profiles");
        Directory.CreateDirectory(dataFolder);
        Directory.CreateDirectory(actorFolder);
        Directory.CreateDirectory(profilesFolder);
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            
        };
        
        foreach (var file in Directory.GetFiles(actorFolder).Where(x => x.EndsWith(".json") || x.EndsWith(".json5")))
        {
            try
            {
                var actorData = JsonSerializer.Deserialize<DataActor>(File.ReadAllText(file));
                if (actorData != null)
                {
                    ActorTables.Add(actorData.Id, actorData);
                    Console.WriteLine($"[Load]: {actorData.Id} {actorData.Name}");
                }
            }
            catch (JsonException jsonEx)
            {
                // 输出 JSON 相关异常的详细信息
                Console.WriteLine($"[JSON Deserialize Error in {file}]:");
                Console.WriteLine($"  Message: {jsonEx.Message}");
                Console.WriteLine($"  Path: {jsonEx.Path}");
                Console.WriteLine($"  LineNumber: {jsonEx.LineNumber}");
                Console.WriteLine($"  BytePositionInLine: {jsonEx.BytePositionInLine}");
                Console.WriteLine($"  StackTrace: {jsonEx.StackTrace}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Deserialize Error]: {e.Message} {e.StackTrace}");
            }
        }

        foreach (var file in Directory.GetFiles(profilesFolder).Where(x => x.EndsWith(".json")))
        {
            try
            {
                var profileData = JsonSerializer.Deserialize<Profile>(File.ReadAllText(file));
                if (profileData != null)
                {
                    ProfileTables.Add(profileData.Account, profileData);
                    Console.WriteLine($"[Load]: {profileData.Account} 资源: {profileData.Items.Count}");
                }
            }
            catch (JsonException jsonEx)
            {
                // 输出 JSON 相关异常的详细信息
                Console.WriteLine($"[JSON Deserialize Error in {file}]:");
                Console.WriteLine($"  Message: {jsonEx.Message}");
                Console.WriteLine($"  Path: {jsonEx.Path}");
                Console.WriteLine($"  LineNumber: {jsonEx.LineNumber}");
                Console.WriteLine($"  BytePositionInLine: {jsonEx.BytePositionInLine}");
                Console.WriteLine($"  StackTrace: {jsonEx.StackTrace}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Deserialize Error]: {e.Message} {e.StackTrace}");
            }
        }
    }
    
    public Dictionary<string, DataActor> ActorTables { get; init; } = new();
    public Dictionary<string, Profile> ProfileTables { get; init; } = new();


    public void Init()
    {
        
    }

    public void Reset()
    {
        
    }
}