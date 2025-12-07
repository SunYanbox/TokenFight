using System.Text.Json;
using TokenFight.Core.Constants;
using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Databases.Models;
using TokenFight.Core.Databases.Models.Dungeons;
using TokenFight.Core.Databases.Models.Profiles;

namespace TokenFight.Core.Databases;


public class DatabaseServer: IDatabaseServer
{
    private const string LogLoadPrefix = "[DatabaseServer Load]";
    
    public DatabaseServer()
    {
        var dataFolder = GameConst.DataFolder;
        var actorFolder = Path.Combine(dataFolder, "actors");
        var profilesFolder = Path.Combine(dataFolder, "profiles");
        var dungeonFolder = Path.Combine(dataFolder, "dungeons");
        Directory.CreateDirectory(dataFolder);
        Directory.CreateDirectory(actorFolder);
        Directory.CreateDirectory(profilesFolder);
        Directory.CreateDirectory(dungeonFolder);
        
        foreach (var file in Directory.GetFiles(actorFolder).Where(x => x.EndsWith(".json")))
        {
            try
            {
                var actorData = JsonSerializer.Deserialize<DataActor>(File.ReadAllText(file));
                if (actorData != null)
                {
                    ActorTables.Add(actorData.Id, actorData);
                    Console.WriteLine($"{LogLoadPrefix}: {actorData.Id} {actorData.Name}");
                }
            }
            catch (JsonException jsonEx)
            {
                // 输出 JSON 相关异常的详细信息
                Console.WriteLine($"[JSON在{file}文件中反序列化错出错]:");
                Console.WriteLine($"  对错误的描述: {jsonEx.Message}");
                Console.WriteLine($"  文件路径: {jsonEx.Path}");
                Console.WriteLine($"  行数: {jsonEx.LineNumber}");
                Console.WriteLine($"  发生异常之前当前行中已读取的以零为起点的字节数: {jsonEx.BytePositionInLine}");
                Console.WriteLine($"  调用堆栈即时帧: {jsonEx.StackTrace}");
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
                    Console.WriteLine($"{LogLoadPrefix}: {profileData.Account} 资源: {profileData.Items.Count}");
                }
            }
            catch (JsonException jsonEx)
            {
                // 输出 JSON 相关异常的详细信息
                Console.WriteLine($"[JSON在{file}文件中反序列化错出错]:");
                Console.WriteLine($"  对错误的描述: {jsonEx.Message}");
                Console.WriteLine($"  文件路径: {jsonEx.Path}");
                Console.WriteLine($"  行数: {jsonEx.LineNumber}");
                Console.WriteLine($"  发生异常之前当前行中已读取的以零为起点的字节数: {jsonEx.BytePositionInLine}");
                Console.WriteLine($"  调用堆栈即时帧: {jsonEx.StackTrace}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Deserialize Error]: {e.Message} {e.StackTrace}");
            }
        }
        
        foreach (var file in Directory.GetFiles(dungeonFolder).Where(x => x.EndsWith(".json")))
        {
            try
            {
                var dungeonInfo = JsonSerializer.Deserialize<DungeonInfo>(File.ReadAllText(file));
                if (dungeonInfo != null)
                {
                    DungeonInfoTables.Add(dungeonInfo.Id, dungeonInfo);
                    Console.WriteLine($"{LogLoadPrefix}: {dungeonInfo.Name}({dungeonInfo.Id}) {dungeonInfo.Desc} " +
                                      $"收益: {dungeonInfo.BaseToken}+{dungeonInfo.PerRandomToken}/敌人 " +
                                      $"敌人数量: {dungeonInfo.EnemyPool.Length}");
                }
            }
            catch (JsonException jsonEx)
            {
                // 输出 JSON 相关异常的详细信息
                Console.WriteLine($"[JSON在{file}文件中反序列化错出错]:");
                Console.WriteLine($"  对错误的描述: {jsonEx.Message}");
                Console.WriteLine($"  文件路径: {jsonEx.Path}");
                Console.WriteLine($"  行数: {jsonEx.LineNumber}");
                Console.WriteLine($"  发生异常之前当前行中已读取的以零为起点的字节数: {jsonEx.BytePositionInLine}");
                Console.WriteLine($"  调用堆栈即时帧: {jsonEx.StackTrace}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"[Deserialize Error]: {e.Message} {e.StackTrace}");
            }
        }
    }
    
    public Dictionary<string, DataActor> ActorTables { get; init; } = new();
    public Dictionary<string, Profile> ProfileTables { get; init; } = new();
    public Dictionary<string, DungeonInfo> DungeonInfoTables { get; init; } = new();


    public void Init()
    {
        
    }

    public void Reset()
    {
        
    }
}