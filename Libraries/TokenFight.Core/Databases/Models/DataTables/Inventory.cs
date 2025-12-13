using System.Text.Json.Serialization;
using TokenFight.Core.Databases.Helpers;
using TokenFight.Core.Databases.Models.Profiles;

namespace TokenFight.Core.Databases.Models.DataTables;

public class Inventory
{
    public Inventory() { }

    [JsonPropertyName("Items")]
    public Item[]? Items
    {
        get => _allItems.Values.ToArray();
        set
        {
            _allItems.Clear();
            if (value == null) return;
            foreach (Item item in value)
            {
                Add(item);
            }
        }
    }
    [JsonIgnore]
    private readonly DataTable<Item> _allItems = new();
    [JsonIgnore]
    private readonly Dictionary<ItemType, DataTable<Item>> _itemTypeTables = Enum.GetValues<ItemType>()
        .ToDictionary(
            key => key,
            _ => new DataTable<Item>()
        );

    #region 批量获取
    /// <summary>
    /// 获取物品
    /// </summary>
    public DataTable<Item> GetItemsByType(ItemType itemType) => _itemTypeTables[itemType];

    [JsonIgnore]
    public DataTable<Item> AllItems => _allItems;

    [JsonIgnore]
    public DataTable<Item> AllWeapons => GetItemsByType(ItemType.Weapon);

    [JsonIgnore]
    public DataTable<Item> AllRelics => GetItemsByType(ItemType.Relics);

    [JsonIgnore]
    public DataTable<Item> AllActors => GetItemsByType(ItemType.Actor);

    [JsonIgnore]
    public DataTable<Item> AllGifts => GetItemsByType(ItemType.Gift);

    [JsonIgnore]
    public DataTable<Item> AllResources => GetItemsByType(ItemType.Resource);

    [JsonIgnore]
    public int Count => _allItems.Count;
    #endregion

    #region 增删改查
    public Item this[string key]
    {
        get
        {
            Item? item = Get(key);
            return item ?? throw new KeyNotFoundException($"物品{key}不存在");
        }
        set => Add(value);
    }

    /// <summary>
    /// 获取物品
    /// </summary>
    public Item? Get(string id) => _allItems.GetValueOrDefault(id);

    /// <summary>
    /// 添加物品
    /// </summary>
    public bool Add(Item item)
    {
        bool tag = false;
        if (!ItemHelper.VerifyItem(item)) return tag;

        tag = _allItems.TryAdd(item.Id!, item);

        if (!tag) return false;

        tag = tag && _itemTypeTables[item.Type].TryAdd(item.Id!, item);

        if (tag) return tag;
        _allItems.Remove(item.Id!);
        return false;
    }

    /// <summary>
    /// 移除物品
    /// </summary>
    public bool Remove(string id)
    {
        if (!_allItems.TryGetValue(id, out Item? item)) return false;
        return _itemTypeTables[item.Type].Remove(id) && _allItems.Remove(id);
    }
    #endregion

}