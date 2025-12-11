using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using TokenFight.Core.Constants;
using TokenFight.Core.Databases.Interfaces;
using TokenFight.Core.Databases.Models;
using TokenFight.Core.Databases.Models.Dungeons;
using TokenFight.Core.Databases.Models.Profiles;

namespace TokenFight.Core.Databases;


public sealed class DatabaseServer: IDatabaseServer
{
    private const string LogLoadPrefix = "[DatabaseServer Load]";
    private const string DataFolder = GameConst.DataFolder;
    private string ActorFolder => Path.Combine(DataFolder, "actors");
    private string ProfileFolder => Path.Combine(DataFolder, "profiles");
    private string DungeonFolder => Path.Combine(DataFolder, "dungeons");
    private readonly JsonSerializerOptions _jsonSaveOption = new()
    {
        WriteIndented = true,
        IndentSize = 2
    };
    
    public DatabaseServer()
    {
        Directory.CreateDirectory(DataFolder);
        Directory.CreateDirectory(ActorFolder);
        Directory.CreateDirectory(ProfileFolder);
        Directory.CreateDirectory(DungeonFolder);
        
        foreach (var file in Directory.GetFiles(ActorFolder).Where(x => x.EndsWith(".json")))
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

        foreach (var file in Directory.GetFiles(ProfileFolder).Where(x => x.EndsWith(".json")))
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
        
        foreach (var file in Directory.GetFiles(DungeonFolder).Where(x => x.EndsWith(".json")))
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
    public Profile? CurrentProfile { get; set; }
    
    private readonly PasswordHasher<string> _passwordHasher = new();
    public bool TryLogin(string account, string password)
    {
        if (ProfileTables.TryGetValue(account, out Profile? profile))
        {
            if (_passwordHasher.VerifyHashedPassword(account, profile.Password, password) ==
                PasswordVerificationResult.Success)
            {
                CurrentProfile = profile;
                return true;
            }
        }

        return false;
    }

    public bool Register(string account, string password)
    {
        if (ProfileTables.ContainsKey(account))
        {
            return false;
        }
        ProfileTables.Add(account, new Profile
        {
            Account = account,
            Password = _passwordHasher.HashPassword(account, password),
            Token = 1600,
            Items = []
        });
        return true;
    }

    public bool Save<T>(T data)
    {
        if (data is Profile profile)
        {
            return Save(profile);
        }
        return false;
    }

    private bool Save(Profile profile)
    {
        try
        {
            var path = Path.Combine(ProfileFolder, $"{profile.Account}.json");
            File.WriteAllText(path, JsonSerializer.Serialize(profile, _jsonSaveOption));
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine($"[{GetType().Name} Save Error] {e.Message} {e.StackTrace}");
            return false;
        }
    }


    public void Init()
    {
        
    }

    public void Reset()
    {
        
    }
}