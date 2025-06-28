namespace ExpressionToSqlWhereClause.ExpressionTree;

/// <summary>
/// 表达式解析的结果
/// </summary>
public class ClauseParserResult
{
    public string WhereClause { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public WhereClauseAdhesive Adhesive { get; set; }
}