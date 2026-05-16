namespace TokenFight.Core.Databases.Models.Profiles;

public class Item
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? TemplateId { get; set; }
    public ItemType Type { get; set; }
    public string? Parent { get; set; }
    public Properties? Properties { get; set; }
}
