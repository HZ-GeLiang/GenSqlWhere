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
        Adhesive = searchCondition.ParseResult?.Adhesive;
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

        Dictionary<string, string> parameters = new Dictionary<string, string>();


        for (int i = matches.Count - 1; i >= 0; i--) //要倒序替换
        {
            string sqlParameterName = matches[i].Value;
            if (Parameters.ContainsKey(sqlParameterName) == false)
            {
                throw new Exception($"参数对象'{nameof(Parameters)}'中不存在名为'{sqlParameterName}'的key值");
            }
            var sqlParameterValue = Parameters[sqlParameterName];
            string sqlParameterValue_str;

            if (sqlParameterValue == null)
            {
                sqlParameterValue_str = "Null";
                //WhereClause = WhereClauseHelper.Replace_WhereClause(WhereClause, sqlParameterName, sqlParameterValue_str);
                parameters[sqlParameterName] = sqlParameterValue_str;
                continue;
            }

            // else sqlParameterValue != null
            var sqlParameterValueType = sqlParameterValue.GetType();
            if (sqlParameterValueType == typeof(string))
            {
                //m,[fn],类型_string
                sqlParameterValue_str = Get_sqlParameterValue_str(sqlParameterName, (string)sqlParameterValue);
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
                    sqlParameterValue_str = $"'{((DateTime)sqlParameterValue).ToString(format)}'";
                }
                else
                {
                    sqlParameterValue_str = $"'{sqlParameterValue}'";
                }
            }
            else if (sqlParameterValueType == typeof(Guid) || sqlParameterValueType == typeof(Guid?))
            {
                //m,[fn],类型_guid
                sqlParameterValue_str = $"'{sqlParameterValue}'";
            }
            else if (sqlParameterValueType == typeof(bool) || sqlParameterValueType == typeof(bool?))
            {
                //m,[fn],类型_bool
                sqlParameterValue_str = $"{(object.Equals(sqlParameterValue, true) == true ? 1 : 0)}";
            }
            else
            {
                sqlParameterValue_str = $"{sqlParameterValue}";
            }

            //WhereClause = WhereClauseHelper.Replace_WhereClause(WhereClause, sqlParameterName, sqlParameterValue_str);
            parameters[sqlParameterName] = sqlParameterValue_str;
        }

        var result = ReplaceSqlParametersWithRegex(WhereClause, parameters)?.TrimEnd();
        return result;
    }

    public static string ReplaceSqlParametersWithRegex(string clause, Dictionary<string, string> parameters)
    {
        string pattern = @"@(\w+)";
        return Regex.Replace(clause, pattern, match =>
        {
            string paramName = match.Groups[1].Value;

            if (parameters.TryGetValue("@" + paramName, out string value))
            {
                return value;
            }
            // 如果没有找到参数，保持原样
            return match.Value;
        });
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