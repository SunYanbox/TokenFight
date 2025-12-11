using TokenFight.Core.Databases.Models.Profiles;
using TokenFight.Core.Models;
using TokenFight.Core.ReflectionAttribute;

namespace TokenFight.Core.Databases.Helpers;

[AutoSysRegistryInit]
public static class ItemHelper
{
    public static GameSystemRegistry? GameSystemRegistry { private get; set; }
    /// <summary> 验证物品是否有效 </summary>
    public static bool VerifyItem(Item item)
    {
        if (item.Id == null || item.Name == null || item.Properties == null) return false;
        switch (item.Type)
        {
            case ItemType.Gift:
                if (item.Properties?.GiftCode == null
                    || item.Properties?.GiftMaxExchangeTimes == null
                    || item.Properties?.UseCallback == null)
                    return false;
                break;
            case ItemType.Weapon:
                if (item.Properties?.SkillId == null 
                    || item.Properties?.WeaponLayers == null
                    || item.Properties?.WeaponLevel == null)
                    return false;
                break;
            case ItemType.Relics:
                if (item.Properties?.RelicsTypeId == null
                    || item.Properties?.RelicsLevel == null
                    || item.Properties?.MainEntry == null
                    || item.Properties?.SubEntries == null)
                    return false;
                break;
            case ItemType.Actor:
                if (item.Properties?.ActorId == null
                    || item.Properties?.ActorLevel == null
                    || item.Properties?.SkillLevel == null)
                    return false;
                break;
            case ItemType.Resource:
                if (item.Properties?.CurrentResources == null
                    || item.Properties?.MaxResources == null)
                    return false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(item.Type), item.Type, null);
        }
        return true;
    }

    public static void InitItem(ItemType itemType, Properties properties, string? parentId = null)
    {
        string uuid = Guid.NewGuid().ToString();
        Item item = new Item
        {
            Id = uuid,
            Name = properties.Name,
            Type = itemType,
            Parent = parentId,
            TemplateId = properties.Id,
            Properties = new Properties(properties)
        };
    }
}