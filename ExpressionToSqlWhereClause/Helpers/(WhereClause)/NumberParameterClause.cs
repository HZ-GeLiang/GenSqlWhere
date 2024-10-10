using System.Text.RegularExpressions;

namespace ExpressionToSqlWhereClause.Helpers;

/// <summary>
/// 条件语句的参数是数字形式的:sql参数名转成数字的
/// </summary>
public class NumberParameterClause
{
    private string _whereClause;

    public NumberParameterClause(string whereClause)
    {
        _whereClause = whereClause;
    }

    /// <summary>
    /// 条件语句的参数是数字形式的:sql参数名转成数字的
    /// </summary>
    /// <returns></returns>
    public string GetNumberParameterClause()
    {
        if (string.IsNullOrWhiteSpace(_whereClause))
        {
            return "";
        }
        _whereClause += WhereClauseHelper.space1;//为了解决替换时出现的 属性名存在包含关系, 示例: ExpressionDemo_属性名存在包含关系.cs

        string pattern = "@[a-zA-Z0-9_]*";
        var matches = Regex.Matches(_whereClause, pattern);
        //要倒序替换
        for (int i = matches.Count - 1; i >= 0; i--)
        {
            WhereClauseHelper.replace_whereClause(ref _whereClause, matches[i].Value, "@" + i);
        }
        return _whereClause.TrimEnd();
    }
}