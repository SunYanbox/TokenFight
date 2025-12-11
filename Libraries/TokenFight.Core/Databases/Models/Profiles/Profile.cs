namespace TokenFight.Core.Databases.Models.Profiles;

public class Profile
{
    /// <summary> 账户 </summary>
    public required string Account { get; set; }
    /// <summary> 密码 </summary>
    public required string Password { get; set; }
    /// <summary> 账户养成资源 </summary>
    public required long Token { get; set; }
    /// <summary> 角色 / 武器 / 光锥 </summary>
    public required List<Item> Items { get; set; }
    /// <summary> 已兑换礼物信息 </summary>
    public required Dictionary<string, int> GiftInfos { get; set; }
}