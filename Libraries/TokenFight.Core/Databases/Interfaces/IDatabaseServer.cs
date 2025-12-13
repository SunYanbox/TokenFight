using TokenFight.Core.Databases.Models.DataTables;
using TokenFight.Core.Databases.Models.Dungeons;
using TokenFight.Core.Databases.Models.Growth;
using TokenFight.Core.Databases.Models.Profiles;
using TokenFight.Core.Interfaces.Bases;
using TokenFight.Core.Models.Build;

namespace TokenFight.Core.Databases.Interfaces;

public interface IDatabaseServer: ISystem
{
    public DataTable<DataActor> ActorTables { get; init; }
    public DataTable<Profile> ProfileTables { get; init; }
    public DataTable<DungeonInfo> DungeonInfoTables { get; init; }
    public DataTable<Properties> TemplateTables { get; init; }
    public DataTable<GachaReward> GachaRewardTables { get; init; }
    public Profile? CurrentProfile { get; set; }
    /// <summary>
    /// 尝试登录
    /// </summary>
    /// <returns>登录是否成功</returns>
    bool TryLogin(string account, string password);
    /// <summary>
    /// 注册新账户
    /// </summary>
    bool Register(string account, string password);
    /// <summary>
    /// 保存数据库信息的接口
    /// </summary>
    public bool Save<T>(T data);
}