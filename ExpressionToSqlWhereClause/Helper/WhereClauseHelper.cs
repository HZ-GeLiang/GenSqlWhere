using ExpressionToSqlWhereClause.ExtensionMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace ExpressionToSqlWhereClause.Helper
{
    /// <summary>
    /// WhereClause的帮助类
    /// </summary>
    public sealed class WhereClauseHelper
    {
        private const string space1 = " ";//一个空格符号

        #region Clause_NonParameter

        /// <inheritdoc cref="GetNonParameterClause(string, Dictionary{string, object}, Dictionary{string, string})"/>
        public static string GetNonParameterClause(SearchCondition searchCondition)
        {
            Dictionary<string, string> formatDateTime = GetDefaultFormatDateTime(searchCondition.Parameters);
            var whreCaluse = GetNonParameterClause(searchCondition.WhereClause, searchCondition.Parameters, formatDateTime);
            return whreCaluse;
        }

        /// <inheritdoc cref="GetNonParameterClause(string, Dictionary{string, object}, Dictionary{string, string})"/>
        public static string GetNonParameterClause(SearchCondition searchCondition, Dictionary<string, string> formatDateTime)
        {
            var whreCaluse = GetNonParameterClause(searchCondition.WhereClause, searchCondition.Parameters, formatDateTime);
            //searchCondition.SetWhereClause(whreCaluse);
            return whreCaluse;
        }

        /// <inheritdoc cref="GetNonParameterClause(string, Dictionary{string, object}, Dictionary{string, string})"/>
        public static string GetNonParameterClause(string whereClause, Dictionary<string, object> parameters)
        {
            Dictionary<string, string> formatDateTime = GetDefaultFormatDateTime(parameters);
            var whreCaluse = GetNonParameterClause(whereClause, parameters, formatDateTime);
            return whreCaluse;
        }

        /// <summary>
        /// 非参数化的条件语句: sql参数合并到sql语句中
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="parameters"></param>
        /// <param name="formatDateTime"></param>
        /// <returns></returns>
        public static string GetNonParameterClause(string whereClause, Dictionary<string, object> parameters, Dictionary<string, string> formatDateTime)
        {
            if (string.IsNullOrWhiteSpace(whereClause))
            {
                return null;
            }
            whereClause += space1;//为了解决替换时出现的 属性名存在包含关系, 示例: ExpressionDemo_属性名存在包含关系.cs
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
                    whereClause = whereClause.Replace(sqlParameterName + space1, "Null" + space1);
                }
                else
                {
                    var sqlParameterValueType = sqlParameterValue.GetType();
                    if (sqlParameterValueType == typeof(string))
                    {
                        if (((string)sqlParameterValue).Contains("'"))
                        {
                            sqlParameterValue = ((string)sqlParameterValue).Replace("'", "''");
                        }

                        whereClause = whereClause.Replace(sqlParameterName + space1, $"'{(string)sqlParameterValue}'" + space1);
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
                            whereClause = whereClause.Replace(sqlParameterName + space1, newVal + space1);
                        }
                        else
                        {
                            whereClause = whereClause.Replace(sqlParameterName + space1, $"'{sqlParameterValue}'" + space1);
                        }
                    }
                    else if (sqlParameterValueType == typeof(Guid) || sqlParameterValueType == typeof(Guid?))
                    {
                        whereClause = whereClause.Replace(sqlParameterName + space1, $"'{sqlParameterValue}'" + space1);
                    }
                    else if (sqlParameterValueType == typeof(bool) || sqlParameterValueType == typeof(bool?))
                    {
                        whereClause = whereClause.Replace(sqlParameterName + space1, $"{(object.Equals(sqlParameterValue, true) == true ? 1 : 0)}" + space1);
                    }
                    else
                    {
                        whereClause = whereClause.Replace(sqlParameterName + space1, $"{sqlParameterValue}" + space1);
                    }
                }
            }
            return whereClause.TrimEnd();
        }

        #endregion

        #region Clause_NumberParameter

        /// <inheritdoc cref="GetNumberParameterClause(string)"/>
        public static string GetNumberParameterClause(SearchCondition searchCondition)
        {
            var whreCaluse = GetNumberParameterClause(searchCondition.WhereClause);
            //searchCondition.SetWhereClause(whreCaluse);
            return whreCaluse;
        }

        /// <summary>
        /// 条件语句的参数是数字形式的:sql参数名转成数字的
        /// </summary>
        /// <param name="whereClause"></param>
        /// <returns></returns>
        public static string GetNumberParameterClause(string whereClause)
        {
            if (string.IsNullOrWhiteSpace(whereClause))
            {
                return "";
            }

            whereClause += space1;//为了解决替换时出现的 属性名存在包含关系, 示例: ExpressionDemo_属性名存在包含关系.cs

            string pattern = "@[a-zA-Z0-9_]*";
            var matches = Regex.Matches(whereClause, pattern);
            //要倒序替换
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                whereClause = whereClause.Replace(matches[i].Value + space1, "@" + i + space1);
            }
            return whereClause.TrimEnd();
        }

        #endregion

        #region Clause_FormattableString

        /// <summary>
        /// 转换为 FormattableString 的对象
        /// </summary>
        /// <param name="searchCondition"></param>
        /// <returns></returns>
        public static string GetFormattableStringClause(SearchCondition searchCondition)
        {
            //这个方法和 GetNumberParameterClause 基本一致, 目前唯一的区别在 whereClause.Replace 的处理
            string whereClause = searchCondition.WhereClause;

            if (string.IsNullOrWhiteSpace(whereClause))
            {
                return "";
            }

            whereClause += space1;//为了解决替换时出现的 属性名存在包含关系, 示例: ExpressionDemo_属性名存在包含关系.cs
            string pattern = "@[a-zA-Z0-9_]*";
            var matches = Regex.Matches(whereClause, pattern);

            for (int i = matches.Count - 1; i >= 0; i--) //要倒序替换
            {
                var sqlParameterName = matches[i].Value;
                whereClause = whereClause.Replace(sqlParameterName + space1, "{" + i + "}" + space1);

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

            return whereClause.TrimEnd();
        }

        public static FormattableString CreateFormattableString(string querySql, object[] FormattableParameters)
        {
            var formattableStr = FormattableStringFactory.Create(querySql, FormattableParameters);
            return formattableStr;
        }

        #endregion

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
                    var type = parameter.Value.GetType();
                    if (type == typeof(DateTime) || type == typeof(DateTime?))
                    {
                        formatDateTime.Add(parameter.Key, format);
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