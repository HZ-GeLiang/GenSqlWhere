using ExpressionToSqlWhereClause.ExpressionTree;
using ExpressionToSqlWhereClause.Helpers;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause;

public class SearchCondition
{
    public SearchCondition()
    {
    }

    public SearchCondition(ClauseParserResult parseResult)
    {
        ParseResult = parseResult;
        WhereClause = TrimParentheses(parseResult.WhereClause);

        {
            //init Parameters
            WhereClauseAdhesive Adhesive = parseResult.Adhesive;
            foreach (var key in Adhesive.GetParameterKeys())
            {
                this.Parameters.Add(key, Adhesive.GetParameter(key).Value);
            }
        }
    }

    public ClauseParserResult ParseResult { get; private set; }

    /// <summary>
    /// where子句(不含where关键字)
    /// </summary>
    public string WhereClause { get; private set; } = "";

    /// <summary>
    /// 查询参数
    /// </summary>
    public Dictionary<string, object> Parameters { get; private set; } = new Dictionary<string, object>();

    /// <summary>
    /// 未加工处理的 WhereClause
    /// </summary>
    public string WhereClauseRaw => ParseResult?.WhereClause ?? "";

    /// <summary>
    /// 去掉()
    /// </summary>
    /// <param name="WhereClause"></param>
    /// <returns></returns>
    private static string TrimParentheses(string WhereClause)
    {
        var canTrimAll = ParenthesesTrimHelper.CanTrimAll(WhereClause, str =>
        {
            //CanTrimAll的基础上 在增加一个, 如果全部为Or 的, 那么可以去掉所有的 ()
            return str.Contains(SqlKeys.And) == false;
        });

        if (canTrimAll) //去掉 关系条件 全为 and 时的 ()
        {
            var str = ParenthesesTrimHelper.TrimAll(WhereClause, new char[] { '(', ')' });//全是and 的
            return str;
        }
        else
        {
            var str = ParenthesesTrimHelper.ParseTrimAll(WhereClause);
            return str;
        }
    }
}