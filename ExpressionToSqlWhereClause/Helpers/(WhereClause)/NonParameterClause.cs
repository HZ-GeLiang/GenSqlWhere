using ExpressionToSqlWhereClause.ExpressionTree;
using System.Text.RegularExpressions;

namespace ExpressionToSqlWhereClause.Helpers;

public class NonParameterClause
{
    private readonly SearchCondition _searchCondition;
    private readonly Dictionary<string, string> _formatDateTime;

    public NonParameterClause(SearchCondition searchCondition)
    {
        _searchCondition = searchCondition;
        _formatDateTime = WhereClauseHelper.GetDefaultFormatDateTime(searchCondition.Parameters);
    }

    public NonParameterClause(SearchCondition searchCondition, Dictionary<string, string> formatDateTime)
    {
        _searchCondition = searchCondition;
        _formatDateTime = formatDateTime;
    }

    /// <summary>
    /// 非参数化的条件语句: sql参数合并到sql语句中
    /// </summary>
    /// <returns></returns>
    public string GetNonParameterClause()
    {
        if (string.IsNullOrWhiteSpace(_searchCondition.WhereClause))
        {
            return "";
        }
        string whereClause = _searchCondition.WhereClause;
        Dictionary<string, object> parameters = _searchCondition.Parameters;
        whereClause += WhereClauseHelper.space1;//为了解决替换时出现的 属性名存在包含关系, 示例: ExpressionDemo_属性名存在包含关系.cs
        var default_formatDateTime = WhereClauseHelper.GetDefaultFormatDateTime(parameters);

        string pattern = "@[a-zA-Z0-9_]*";
        var matches = Regex.Matches(whereClause, pattern);

        if (matches.Count > 0 && parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        for (int i = matches.Count - 1; i >= 0; i--) //要倒序替换
        {
            string sqlParameterName = matches[i].Value;
            if (parameters.ContainsKey(sqlParameterName) == false)
            {
                throw new Exception($"参数对象'{nameof(parameters)}'中不存在名为'{sqlParameterName}'的key值");
            }
            var sqlParameterValue = parameters[matches[i].Value];

            if (sqlParameterValue == null)
            {
                WhereClauseHelper.replace_whereClause(ref whereClause, sqlParameterName, "Null");
            }
            else
            {
                var sqlParameterValueType = sqlParameterValue.GetType();
                if (sqlParameterValueType == typeof(string))
                {
                    var sqlParameterValue_str = (string)sqlParameterValue;
                    var pmsInfo = _searchCondition.ParseResult.Adhesive.GetParameter(sqlParameterName);
                    if (pmsInfo.Symbol == SqlKeys.Equal && sqlParameterValue_str.Contains("'"))
                    {
                        sqlParameterValue_str = $"'{sqlParameterValue_str.Replace("'", "''")}'";
                    }
                    else if (pmsInfo.Symbol == SqlKeys.@in)
                    {
                        sqlParameterValue_str = $"{sqlParameterValue_str}";
                    }
                    else
                    {
                        sqlParameterValue_str = $"'{sqlParameterValue_str}'";
                    }

                    WhereClauseHelper.replace_whereClause(ref whereClause, sqlParameterName, sqlParameterValue_str);
                }
                else if (sqlParameterValueType == typeof(DateTime) || sqlParameterValueType == typeof(DateTime?))
                {
                    string format = null;
                    if (_formatDateTime != null && _formatDateTime.ContainsKey(sqlParameterName))
                    {
                        format = _formatDateTime[sqlParameterName]; //取用户配置的
                    }
                    else if (default_formatDateTime.ContainsKey(sqlParameterName))
                    {
                        format = default_formatDateTime[sqlParameterName];//取默认配置
                    }

                    if (!string.IsNullOrWhiteSpace(format))
                    {
                        var newVal = $"'{((DateTime)sqlParameterValue).ToString(format)}'";
                        WhereClauseHelper.replace_whereClause(ref whereClause, sqlParameterName, newVal);
                    }
                    else
                    {
                        WhereClauseHelper.replace_whereClause(ref whereClause, sqlParameterName, $"'{sqlParameterValue}'");
                    }
                }
                else if (sqlParameterValueType == typeof(Guid) || sqlParameterValueType == typeof(Guid?))
                {
                    WhereClauseHelper.replace_whereClause(ref whereClause, sqlParameterName, $"'{sqlParameterValue}'");
                }
                else if (sqlParameterValueType == typeof(bool) || sqlParameterValueType == typeof(bool?))
                {
                    WhereClauseHelper.replace_whereClause(ref whereClause, sqlParameterName, $"{(object.Equals(sqlParameterValue, true) == true ? 1 : 0)}");
                }
                else
                {
                    WhereClauseHelper.replace_whereClause(ref whereClause, sqlParameterName, $"{sqlParameterValue}");
                }
            }
        }
        return whereClause.TrimEnd();
    }
}