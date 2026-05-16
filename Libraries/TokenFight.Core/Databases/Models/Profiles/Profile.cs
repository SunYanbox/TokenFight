using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using TokenFight.Core.Databases.Helpers;
using TokenFight.Core.Databases.Models.DataTables;
using TokenFight.Core.Models.Build;

namespace TokenFight.Core.Databases.Models.Profiles;

public class Profile(
    string account,
    string password,
    long token,
    Inventory inventory,
    Dictionary<string, int> giftInfos,
    GachaHistory gachaHistory)
{
    /// <summary> 账户 </summary>
    public required string Account { get; set; } = account;
    /// <summary> 密码 </summary>
    public required string Password { get; set; } = password;
    /// <summary> 账户养成资源 </summary>
    public required long Token { get; set; } = token;
    /// <summary> 角色 / 武器 / 光锥 </summary>
    public required Inventory Inventory { get; set; } = inventory;
    /// <summary> 已兑换礼物信息 </summary>
    public required Dictionary<string, int> GiftInfos { get; set; } = giftInfos;
    /// <summary> 抽卡历史记录 </summary>
    public required GachaHistory GachaHistory { get; set; } = gachaHistory;


    [JsonIgnore]
    private static readonly PasswordHasher<string> PasswordHasher = new();

    [SetsRequiredMembers]
    public Profile() : this("", "")
    {

    }

    [SetsRequiredMembers]
    public Profile(string account, string password)
        : this(account, PasswordHasher.HashPassword(account, password),
            1600, new Inventory(), new Dictionary<string, int>(), new GachaHistory())
    {
    }

    /// <summary> 添加抽卡结果到物品库存 </summary>
    public void Add(GachaResult result)
    {
        GachaHistory.Add(result);
        for (int i = 0; i < result.Count; i++)
        {
            if (result.ItemTpl == null) continue;
            try
            {
                Item item = ItemHelper.InitItem(result.ItemTpl);
                Inventory.Add(item);
            }
            catch (Exception e)
            {
                Console.WriteLine($"[添加抽卡结果] 添加物品失败: {result.ItemTpl} {e.Message} {e.StackTrace}");
                throw;
            }
        }
    }
}
