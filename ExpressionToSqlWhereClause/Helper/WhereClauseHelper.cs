using ExpressionToSqlWhereClause.ExtensionMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExpressionToSqlWhereClause.Helper
{
    public sealed class WhereClauseHelper
    {
        /// <inheritdoc cref="ParamNameToNumber(string)"/>
        public static void ParamNameToNumber(SearchCondition searchCondition)
        {
            var newWhereClause = ParamNameToNumber(searchCondition.WhereClause);
            searchCondition.SetWhereClause(newWhereClause);
        }

        /// <summary>
        /// sql参数名转成数字的
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public static string ParamNameToNumber(string whereClause)
        {
            if (string.IsNullOrWhiteSpace(whereClause))
            {
                return null;
            }

            string pattern = "@[a-zA-Z0-9_]*";
            var matches = Regex.Matches(whereClause, pattern);
            //要倒序替换
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                whereClause = whereClause.Replace(matches[i].Value, "@" + i);
            }
            return whereClause;
        }

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
            Dictionary<string, string> formatDateTime = new Dictionary<string, string>();
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    var type = parameter.Value.GetType();
                    if (type == typeof(DateTime) || type == typeof(DateTime?))
                    {
                        formatDateTime.Add(parameter.Key, format);
                    }
                }
            }
            return formatDateTime;
        }

        /// <inheritdoc cref="MergeParametersIntoSql(string, Dictionary{string, object}, Dictionary{string, string})"/>
        public static void MergeParametersIntoSql(SearchCondition searchCondition)
        {
            Dictionary<string, string> formatDateTime = GetDefaultFormatDateTime(searchCondition.Parameters);
            var newWhereClause = MergeParametersIntoSql(searchCondition.WhereClause, searchCondition.Parameters, formatDateTime);
            searchCondition.SetWhereClause(newWhereClause);
        }

        /// <inheritdoc cref="MergeParametersIntoSql(string, Dictionary{string, object}, Dictionary{string, string})"/>
        public static void MergeParametersIntoSql(SearchCondition searchCondition, Dictionary<string, string> formatDateTime)
        {
            var newWhereClause = MergeParametersIntoSql(searchCondition.WhereClause, searchCondition.Parameters, formatDateTime);
            searchCondition.SetWhereClause(newWhereClause);
        }

        /// <inheritdoc cref="MergeParametersIntoSql(string, Dictionary{string, object}, Dictionary{string, string})"/>
        public static string MergeParametersIntoSql(string whereClause, Dictionary<string, object> parameters)
        {
            Dictionary<string, string> formatDateTime = GetDefaultFormatDateTime(parameters);
            var whreCaluse = MergeParametersIntoSql(whereClause, parameters, formatDateTime);
            return whreCaluse;
        }

        /// <summary>
        /// sql参数合并到sql语句中
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="parameters"></param>
        /// <param name="formatDateTime"></param>
        /// <returns></returns>
        public static string MergeParametersIntoSql(string whereClause, Dictionary<string, object> parameters, Dictionary<string, string> formatDateTime)
        {
            if (string.IsNullOrWhiteSpace(whereClause))
            {
                return null;
            }

            var default_formatDateTime = WhereClauseHelper.GetDefaultFormatDateTime(parameters);

            string pattern = "@[a-zA-Z0-9_]*";
            var matches = Regex.Matches(whereClause, pattern);

            for (int i = matches.Count - 1; i >= 0; i--) //要倒序替换
            {
                var sqlParameterName = matches[i].Value;
                var sqlParameterValue = parameters[matches[i].Value];

                if (sqlParameterValue == null)
                {
                    whereClause = whereClause.Replace(sqlParameterName, "Null");
                }
                else
                {
                    var sqlParameterValueType = sqlParameterValue.GetType();
                    if (sqlParameterValueType == typeof(string) ||
                        sqlParameterValueType == typeof(Guid) ||
                        sqlParameterValueType == typeof(Guid?))
                    {
                        whereClause = whereClause.Replace(sqlParameterName, $"'{sqlParameterValue}'");
                    }
                    else if (sqlParameterValueType == typeof(DateTime) || sqlParameterValueType == typeof(DateTime?))
                    {
                        string format = null;
                        if (formatDateTime != null && formatDateTime.ContainsKey(sqlParameterName))
                        {
                            format = formatDateTime[sqlParameterName]; //取用户配置的
                        }
                        else if (default_formatDateTime.ContainsKey(sqlParameterName))
                        {
                            format = default_formatDateTime[sqlParameterName];//取默认配置
                        }

                        if (!string.IsNullOrWhiteSpace(format))
                        {
                            var newVal = $"'{((DateTime)sqlParameterValue).ToString(format)}'";
                            whereClause = whereClause.Replace(sqlParameterName, newVal);
                        }
                        else
                        {
                            whereClause = whereClause.Replace(sqlParameterName, $"'{sqlParameterValue}'");
                        }
                    }
                    else
                    {
                        whereClause = whereClause.Replace(sqlParameterName, $"{sqlParameterValue}");
                    }
                }
            }
            return whereClause;
        }

        public static void ToFormattableString(SearchCondition searchCondition)
        {
            var result = ToFormattableString(searchCondition.WhereClause, searchCondition.Parameters);
            searchCondition.SetFormattableStringParameters(result.whereClause, result.parameters);
        }

        public static (string whereClause, object[] parameters) ToFormattableString(string whereClause, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(whereClause))
            {
                return ("", new object[] { });
            }

            string pattern = "@[a-zA-Z0-9_]*";
            var matches = Regex.Matches(whereClause, pattern);

            for (int i = matches.Count - 1; i >= 0; i--) //要倒序替换
            {
                var sqlParameterName = matches[i].Value;
                whereClause = whereClause.Replace(sqlParameterName, "{" + i + "}");

                //var sqlParameterValue = parameters[matches[i].Value];
                //if (sqlParameterValue == null)
                //{
                //    whereClause = whereClause.Replace(sqlParameterName, "{" + i + "}");
                //}
                //else
                //{
                //    var sqlParameterValueType = sqlParameterValue.GetType();
                //    if (sqlParameterValueType == typeof(string) ||
                //        sqlParameterValueType == typeof(DateTime) ||
                //        sqlParameterValueType == typeof(DateTime?) ||
                //        sqlParameterValueType == typeof(Guid) ||
                //        sqlParameterValueType == typeof(Guid?)
                //        )
                //    {
                //        //whereClause = whereClause.Replace(sqlParameterName, "'{" + i + "}'");//不需要这样写
                //        whereClause = whereClause.Replace(sqlParameterName, "{" + i + "}");
                //    }
                //    else
                //    {
                //        whereClause = whereClause.Replace(sqlParameterName, "{" + i + "}");
                //    }
                //}
            }

            var result = (whereClause, parameters?.Values.ToArray() ?? new object[] { });
            return result;
        }


        /// <summary>
        /// 转换参数. 如转成 sqlserver 的参数等等
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <param name="convertFunc">Func的参数:  string key  , object value , T 返回对象 </param>
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
}
