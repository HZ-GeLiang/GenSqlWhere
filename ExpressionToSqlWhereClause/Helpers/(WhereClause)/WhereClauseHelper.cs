using System.Runtime.CompilerServices;

namespace ExpressionToSqlWhereClause;

/// <summary>
/// WhereClause的帮助类
/// </summary>
public sealed class WhereClauseHelper
{
    internal const string space1 = " ";//一个空格符号

    #region Clause_NonParameter

    public static string GetNonParameterClause(SearchCondition searchCondition)
    {
        Dictionary<string, string> formatDateTime = GetDefaultFormatDateTime(searchCondition.Parameters);
        var clause = new NonParameterClause(searchCondition, formatDateTime);
        var whereCaluse = clause.GetNonParameterClause();
        return whereCaluse;
    }

    public static string GetNonParameterClause(SearchCondition searchCondition, Dictionary<string, string> formatDateTime)
    {
        var clause = new NonParameterClause(searchCondition, formatDateTime);
        var whereCaluse = clause.GetNonParameterClause();
        return whereCaluse;
    }

    public static string GetNonParameterClause(string whereClause, Dictionary<string, object> parameters)
    {
        var clause = new NonParameterClause(whereClause, parameters);
        var whereCaluse = clause.GetNonParameterClause();
        return whereCaluse;
    }

    public static string GetNonParameterClause(string whereClause, Dictionary<string, object> parameters, Dictionary<string, string> formatDateTime)
    {
        var clause = new NonParameterClause(whereClause, parameters, formatDateTime);
        var whereCaluse = clause.GetNonParameterClause();
        return whereCaluse;
    }

    internal static string replace_whereClause(string whereClause, string sqlParameterName, string newValue)
    {
        string key;
        //注: in参数的 处理必须要在 常规的参数的判断之前

        {
            //in的参数的前面几个
            key = sqlParameterName + space1 + ",";
            if (whereClause.Contains(key))
            {
                whereClause = whereClause.Replace(key, newValue + ",");
                return whereClause;
            }
        }

        {
            //in的参数的最后一个
            key = sqlParameterName + space1 + ")";
            if (whereClause.Contains(key))
            {
                whereClause = whereClause.Replace(key, newValue + ")");
                return whereClause;
            }
        }

        {
            //常规的参数
            key = sqlParameterName + space1;
            if (whereClause.Contains(key))
            {
                whereClause = whereClause.Replace(key, newValue + space1);
                return whereClause;
            }
        }

        {
            //
            key = "(" + sqlParameterName + ")";
            if (whereClause.Contains(key))
            {
                whereClause = whereClause.Replace(key, "(" + newValue + ")");
                return whereClause;
            }
        }

        return whereClause;
    }

    #endregion

    #region Clause_NumberParameter

    public static string GetNumberParameterClause(SearchCondition searchCondition)
    {
        var clause = new NumberParameterClause(searchCondition.WhereClause).GetNumberParameterClause();
        return clause;
    }

    /// <returns></returns>
    public static string GetNumberParameterClause(string whereClause)
    {
        var clause = new NumberParameterClause(whereClause).GetNumberParameterClause();
        return clause;
    }

    #endregion

    #region Clause_FormattableString

    public static string GetFormattableStringClause(SearchCondition searchCondition)
    {
        var clause = new FormattableStringClause(searchCondition.WhereClause).GetNumberParameterClause();
        return clause;
    }

    #endregion

    public static FormattableString CreateFormattableString(string querySql, object[] FormattableParameters)
    {
        var formattableStr = FormattableStringFactory.Create(querySql, FormattableParameters);
        return formattableStr;
    }

    #region GetDefaultFormatDateTime

    /// <summary>
    /// 默认的日期格式
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetDefaultFormatDateTime(Dictionary<string, object> parameters)
    {
        return GetDefaultFormatDateTime(parameters, "yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 默认的日期格式
    /// </summary>
    /// <param name="parameters"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetDefaultFormatDateTime(Dictionary<string, object> parameters, string format)
    {
        Dictionary<string, string> formatDateTime = new();
        if (parameters != null)
        {
            foreach (var parameter in parameters)
            {
                if (parameter.Value != null)
                {
                    var type = parameter.Value.GetType();
                    if (type == typeof(DateTime) || type == typeof(DateTime?))
                    {
                        formatDateTime.Add(parameter.Key, format);
                    }
                }
            }
        }
        return formatDateTime;
    }

    #endregion

    /// <summary>
    /// 转换参数. 如转成 sqlserver 的参数等等
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters"></param>
    /// <param name="convertFunc">Func的参数:string key, object value , T 返回对象。注：key 以@开头 </param>
    /// <returns></returns>
    public static T[] ConvertParameters<T>(Dictionary<string, object> parameters, Func<string, object, T> convertFunc)
    {
        if (parameters == null || convertFunc == null ||
            !parameters.Keys.Any())
        {
            return Array.Empty<T>();
        }

        T[] tArray = new T[parameters.Count];
        int i = 0;
        foreach (var key in parameters.Keys)
        {
            //key: @Id

            tArray[i] = convertFunc.Invoke(key, parameters[key]);
            i++;
        }
        return tArray;
    }

    /*
    /// <summary>
    /// sqlserver 使用的
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public static System.Data.SqlClient.SqlParameter[] ParamToSqlParameter(Dictionary<string, object> param)
    {
        if (param == null)
        {
            return new System.Data.SqlClient.SqlParameter[0];
        }
        var cmdParms = new System.Data.SqlClient.SqlParameter[param.Keys.Count];
        var i = 0;
        foreach (var key in param.Keys)
        {
            cmdParms[i] = new System.Data.SqlClient.SqlParameter(key, param[key]);
            i++;
        }
        return cmdParms;
    }
    */
}