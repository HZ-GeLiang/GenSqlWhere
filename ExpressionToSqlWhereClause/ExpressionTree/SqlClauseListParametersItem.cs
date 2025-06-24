namespace ExpressionToSqlWhereClause.ExpressionTree;

public class SqlClauseListParametersItem
{
    public SqlClauseListParametersItem(SqlClauseParametersInfo para)
    {
        this.Key = para.Key;
        this.Value = para.Value;
    }

    /// <summary>
    /// 参数化的key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// 参数化的值
    /// </summary>
    public object Value { get; set; }
}