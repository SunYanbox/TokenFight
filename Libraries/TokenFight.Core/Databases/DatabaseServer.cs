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
    private string TemplateFolder => Path.Combine(DataFolder, "templates");
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
        Directory.CreateDirectory(TemplateFolder);
        
        LoadDataFromFolder(ActorFolder, ActorTables, (actorData, dict) => 
        {
            dict.Add(actorData.Id, actorData);
            Console.WriteLine($"{LogLoadPrefix}: {actorData.Id} {actorData.Name}");
        });
        
        LoadDataFromFolder(ProfileFolder, ProfileTables, (profileData, dict) => 
        {
            dict.Add(profileData.Account, profileData);
            Console.WriteLine($"{LogLoadPrefix}: {profileData.Account} 资源: {profileData.Items.Count}");
        });
        
        LoadDataFromFolder(DungeonFolder, DungeonInfoTables, (dungeonInfo, dict) => 
        {
            dict.Add(dungeonInfo.Id, dungeonInfo);
            Console.WriteLine($"{LogLoadPrefix}: {dungeonInfo.Name}({dungeonInfo.Id}) {dungeonInfo.Desc} " +
                              $"收益: {dungeonInfo.BaseToken}+{dungeonInfo.PerRandomToken}/敌人 " +
                              $"敌人数量: {dungeonInfo.EnemyPool.Length}");
        });
        
        LoadDataFromFolder(TemplateFolder, TemplateTables, (templateData, dict) => 
        {
            dict.Add(templateData.Id, templateData);
            Console.WriteLine($"{LogLoadPrefix}: 加载物品模板: {templateData.Id} {templateData.Name}");
        });
    }
    
    public Dictionary<string, DataActor> ActorTables { get; init; } = new();
    public Dictionary<string, Profile> ProfileTables { get; init; } = new();
    public Dictionary<string, DungeonInfo> DungeonInfoTables { get; init; } = new();
    public Dictionary<string, Properties> TemplateTables { get; init; } = new();
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
        if (string.IsNullOrWhiteSpace(account) || string.IsNullOrWhiteSpace(password)
            || !account.All(char.IsLetterOrDigit) || !password.All(char.IsAsciiLetterOrDigit)
            || account.Length < 4 || account.Length > 16 || password.Length < 4 || password.Length > 16)
        {
            return false;
        }
        if (ProfileTables.ContainsKey(account))
        {
            return false;
        }
        ProfileTables.Add(account, new Profile
        {
            Account = account,
            Password = _passwordHasher.HashPassword(account, password),
            Token = 1600,
            Items = [],
            GiftInfos = new Dictionary<string, int>()
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

    /// <summary>
    /// 从指定文件夹加载JSON数据到字典
    /// </summary>
    /// <typeparam name="T">数据类型</typeparam>
    /// <param name="folderPath">文件夹路径</param>
    /// <param name="dictionary">目标字典</param>
    /// <param name="onSuccess">数据成功加载后的回调</param>
    private void LoadDataFromFolder<T>(string folderPath, Dictionary<string, T> dictionary, Action<T, Dictionary<string, T>> onSuccess) 
        where T : class
    {
        if (!Directory.Exists(folderPath))
            return;
            
        foreach (var file in Directory.GetFiles(folderPath).Where(x => x.EndsWith(".json")))
        {
            try
            {
                var jsonContent = File.ReadAllText(file);
                var data = JsonSerializer.Deserialize<T>(jsonContent);
                
                if (data != null)
                {
                    onSuccess(data, dictionary);
                }
            }
            catch (JsonException jsonEx)
            {
                // 输出 JSON 相关异常的详细信息
                Console.WriteLine($"[JSON在{file}文件中反序列化出错]:");
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

    public void Init()
    {
        
    }

    public void Reset()
    {
        
    }
}