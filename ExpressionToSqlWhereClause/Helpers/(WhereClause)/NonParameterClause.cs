using ExpressionToSqlWhereClause.ExpressionTree;
using System.Text.RegularExpressions;

namespace ExpressionToSqlWhereClause;

public class NonParameterClause
{
    //private readonly SearchCondition _searchCondition;
    public string WhereClause { get; private set; }

    public Dictionary<string, object> Parameters { get; private set; }

    public WhereClauseAdhesive Adhesive { get; private set; }

    public Dictionary<string, string> FormatDateTime { get; private set; }

    public NonParameterClause(SearchCondition searchCondition) : this(searchCondition, null)
    {
    }

    public NonParameterClause(SearchCondition searchCondition, Dictionary<string, string> formatDateTime)
    {
        WhereClause = searchCondition.WhereClause;
        Parameters = searchCondition.Parameters;
        Adhesive = searchCondition.ParseResult.Adhesive;
        if (formatDateTime == null)
        {
            FormatDateTime = WhereClauseHelper.GetDefaultFormatDateTime(searchCondition.Parameters);
        }
        else
        {
            FormatDateTime = formatDateTime;
        }
    }

    public NonParameterClause(string whereClause, Dictionary<string, object> parameters) : this(whereClause, parameters, null)
    {
    }

    public NonParameterClause(string whereClause, Dictionary<string, object> parameters, Dictionary<string, string> formatDateTime)
    {
        WhereClause = whereClause;
        Parameters = parameters;
        Adhesive = null;
        if (formatDateTime == null)
        {
            FormatDateTime = WhereClauseHelper.GetDefaultFormatDateTime(parameters);
        }
        else
        {
            FormatDateTime = formatDateTime;
        }
    }

    /// <summary>
    /// 非参数化的条件语句: sql参数合并到sql语句中
    /// </summary>
    /// <returns></returns>
    public string GetNonParameterClause()
    {
        if (string.IsNullOrWhiteSpace(WhereClause))
        {
            return "";
        }

        WhereClause += WhereClauseHelper.space1;//为了解决替换时出现的 属性名存在包含关系, 示例: 单元测试 : 属性名存在包含关系

        var default_formatDateTime = WhereClauseHelper.GetDefaultFormatDateTime(Parameters);

        string pattern = "@[a-zA-Z0-9_]*";
        var matches = Regex.Matches(WhereClause, pattern);

        if (matches.Count > 0 && Parameters == null)
        {
            throw new ArgumentNullException(nameof(Parameters));
        }

        for (int i = matches.Count - 1; i >= 0; i--) //要倒序替换
        {
            string sqlParameterName = matches[i].Value;
            if (Parameters.ContainsKey(sqlParameterName) == false)
            {
                throw new Exception($"参数对象'{nameof(Parameters)}'中不存在名为'{sqlParameterName}'的key值");
            }
            var sqlParameterValue = Parameters[sqlParameterName];

            if (sqlParameterValue == null)
            {
                WhereClause = WhereClauseHelper.Replace_WhereClause(WhereClause, sqlParameterName, "Null");
            }
            else
            {
                var sqlParameterValueType = sqlParameterValue.GetType();
                if (sqlParameterValueType == typeof(string))
                {
                    //m,[fn],类型_string
                    string sqlParameterValue_str = Get_sqlParameterValue_str(sqlParameterName, (string)sqlParameterValue);

                    WhereClause = WhereClauseHelper.Replace_WhereClause(WhereClause, sqlParameterName, sqlParameterValue_str);
                }
                else if (sqlParameterValueType == typeof(DateTime) || sqlParameterValueType == typeof(DateTime?))
                {
                    string format = null;
                    if (FormatDateTime != null && FormatDateTime.ContainsKey(sqlParameterName))
                    {
                        format = FormatDateTime[sqlParameterName]; //取用户配置的
                    }
                    else if (default_formatDateTime.ContainsKey(sqlParameterName))
                    {
                        format = default_formatDateTime[sqlParameterName];//取默认配置
                    }

                    if (!string.IsNullOrWhiteSpace(format))
                    {
                        var newVal = $"'{((DateTime)sqlParameterValue).ToString(format)}'";
                        WhereClause = WhereClauseHelper.Replace_WhereClause(WhereClause, sqlParameterName, newVal);
                    }
                    else
                    {
                        WhereClause = WhereClauseHelper.Replace_WhereClause(WhereClause, sqlParameterName, $"'{sqlParameterValue}'");
                    }
                }
                else if (sqlParameterValueType == typeof(Guid) || sqlParameterValueType == typeof(Guid?))
                {
                    //m,[fn],类型_guid
                    WhereClause = WhereClauseHelper.Replace_WhereClause(WhereClause, sqlParameterName, $"'{sqlParameterValue}'");
                }
                else if (sqlParameterValueType == typeof(bool) || sqlParameterValueType == typeof(bool?))
                {
                    //m,[fn],类型_bool
                    WhereClause = WhereClauseHelper.Replace_WhereClause(WhereClause, sqlParameterName, $"{(object.Equals(sqlParameterValue, true) == true ? 1 : 0)}");
                }
                else
                {
                    WhereClause = WhereClauseHelper.Replace_WhereClause(WhereClause, sqlParameterName, $"{sqlParameterValue}");
                }
            }
        }
        return WhereClause.TrimEnd();
    }

    private string Get_sqlParameterValue_str(string sqlParameterName, string sqlParameterValue_str)
    {
        if (Adhesive != null)
        {
            SqlClauseParametersInfo pmsInfo = Adhesive.GetParameter(sqlParameterName);
            if (pmsInfo != null)
            {
                if (pmsInfo.Symbol == SqlKeys.Equal && sqlParameterValue_str.Contains("'"))
                {
                    return $"'{sqlParameterValue_str.Replace("'", "''")}'";
                }
                if (pmsInfo.Symbol == SqlKeys.@in)
                {
                    return $"{sqlParameterValue_str}";
                }
            }
        }

        return $"'{sqlParameterValue_str}'";
    }
}