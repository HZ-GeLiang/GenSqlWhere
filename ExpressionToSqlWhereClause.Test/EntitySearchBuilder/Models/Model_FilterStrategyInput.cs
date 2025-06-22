namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

/// <summary>
/// 获取收款input
/// </summary>
public class Model_FilterStrategyInput
{
    public int? Id { get; set; }
    public decimal GetSum { get; set; }

    /// <summary>
    /// 筛选符号
    /// </summary>
    public string GetSumFilter { get; set; }
}