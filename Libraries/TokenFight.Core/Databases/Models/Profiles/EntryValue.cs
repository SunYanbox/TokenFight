using TokenFight.Core.Enums.Attrs;

namespace TokenFight.Core.Databases.Models.Profiles;

public class EntryValue(AttrType attrType, double value)
{
    public AttrType AttrType { get; set; } = attrType;
    public double Value { get; set; } = value;

    public override string ToString() => $"{AttrType}: {Value}";

    public EntryValue(EntryValue? other): this(other?.AttrType ?? AttrType.EndTag, other?.Value ?? 0)
    {
    }
}