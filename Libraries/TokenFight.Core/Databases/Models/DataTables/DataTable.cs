namespace TokenFight.Core.Databases.Models.DataTables;

/// <summary>
/// 数据表
/// </summary>
public class DataTable<TValue> : Dictionary<string, TValue> where TValue : class;
