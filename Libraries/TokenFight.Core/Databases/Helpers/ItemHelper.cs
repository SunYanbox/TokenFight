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
        return VerifyProperties(item.Properties, item.Type);
    }

    public static bool VerifyProperties(Properties properties, ItemType itemType)
    { 
        switch (itemType)
        {
            case ItemType.Gift:
                if (properties.GiftCode == null
                    || properties.GiftMaxExchangeTimes == null
                    || properties.UseCallback == null)
                    return false;
                break;
            case ItemType.Weapon:
                if (properties.SkillId == null 
                    || properties.WeaponLayers == null
                    || properties.WeaponLevel == null)
                    return false;
                break;
            case ItemType.Relics:
                if (properties.RelicsTypeId == null
                    || properties.RelicsLevel == null
                    || properties.MainEntry == null
                    || properties.SubEntries == null)
                    return false;
                break;
            case ItemType.Actor:
                if (properties.ActorId == null
                    || properties.ActorLevel == null
                    || properties.SkillLevel == null)
                    return false;
                break;
            case ItemType.Resource:
                if (properties.CurrentResources == null
                    || properties.MaxResources == null)
                    return false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(itemType), itemType, null);
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

    /// <summary>
    /// 获取已验证的指定类型的ITEMS
    /// </summary>
    public static Item[] GetVerifyItems(Item[] items, ItemType itemType)
    {
        return items.Where(item => item.Type == itemType && VerifyItem(item)).ToArray();
    }
}