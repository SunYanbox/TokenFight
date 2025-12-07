namespace TokenFight.Core.Databases.Models.Profiles;

public struct Item
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Template { get; set; }
    public string? Parent { get; set; }
    public Properties? Properties { get; set; }
}