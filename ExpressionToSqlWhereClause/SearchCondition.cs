using ExpressionToSqlWhereClause.ExpressionTree;
using ExpressionToSqlWhereClause.Helper;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause;

public class SearchCondition
{
    public SearchCondition()
    {
        WhereClauseRaw = string.Empty;
        WhereClause = string.Empty;
        Parameters = new Dictionary<string, object>();
        //this.FormattableParameters = null;
    }

    public SearchCondition(ClauseParserResult parseResult)
    {
        WhereClauseRaw = parseResult.WhereClause;
        WhereClause = TrimParentheses(parseResult.WhereClause);
        Parameters = GetParameters(parseResult.Adhesive);
    }

    /// <summary>
    /// where子句(不含where关键字)
    /// </summary>
    public string WhereClause { get; private set; }

    /// <summary>
    /// 查询参数
    /// </summary>
    public Dictionary<string, object> Parameters { get; private set; }

    /// <summary>
    /// FormattableString 的
    /// </summary>
    //public object[] FormattableParameters { get; private set; }

    /// <summary>
    /// 未加工处理的 WhereClause
    /// </summary>
    public string WhereClauseRaw { get; private set; }

    //public void SetWhereClause(string whereClause)
    //{
    //    this.WhereClause = whereClause;
    //}

    //public void SetFormattableStringParameters(string whereClause, object[] fmtParameters)
    //{
    //    this.WhereClause = whereClause;
    //    this.FormattableParameters = fmtParameters;
    //}

    //public FormattableString GetFormattableString(string querySql)
    //{
    //    var formattableStr = FormattableStringFactory.Create(querySql, this.FormattableParameters);
    //    return formattableStr;
    //}

    // <summary>
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

    /// <summary>
    /// 获得参数
    /// </summary>
    /// <param name="Adhesive"></param>
    /// <returns></returns>
    private static Dictionary<string, object> GetParameters(WhereClauseAdhesive Adhesive)
    {
        var parameters = new Dictionary<string, object>(0);
        foreach (var key in Adhesive.GetParameterKeys())
        {
            parameters.Add(key, Adhesive.GetParameter(key).Value);
        }
        return parameters;
    }
}