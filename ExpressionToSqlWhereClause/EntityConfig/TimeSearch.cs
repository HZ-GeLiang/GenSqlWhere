namespace ExpressionToSqlWhereClause.EntityConfig;

/// <summary>
/// 时间搜索
/// </summary>
internal class TimeSearch
{
    public string Prop { get; set; }

    public DateTime?[] StartAndEnd { get; set; }

    public bool? IsPair { get; set; }
}