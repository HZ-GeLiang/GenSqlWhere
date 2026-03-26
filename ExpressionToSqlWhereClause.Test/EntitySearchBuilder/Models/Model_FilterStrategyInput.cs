namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;


public class Model_FilterStrategyInput
{
    public int? Id { get; set; }

    #region 数值

    public decimal GetSum { get; set; }

    /// <summary>
    /// 筛选符号
    /// </summary>
    public string GetSumFilter { get; set; }

    #endregion

    #region 日期

    public DateTime? CreateDate { get; set; }
    public DateTime? CreateDate2 { get; set; }

    /// <summary>
    /// 筛选符号
    /// </summary>
    public string CreateDateFilter { get; set; }

    #endregion

}