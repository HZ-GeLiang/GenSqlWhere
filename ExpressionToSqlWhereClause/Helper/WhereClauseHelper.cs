using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ExpressionToSqlWhereClause.Helper
{
    public class WhereClauseHelper
    {
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
        /// sql参数合并到sql语句中
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="parameters"></param>
        /// <param name="formatDateTime"></param>
        /// <returns></returns>
        public static string MergeParametersIntoSql(string whereClause, Dictionary<string, object> parameters,
           Dictionary<string, string> formatDateTime = null)
        {
            if (string.IsNullOrWhiteSpace(whereClause))
            {
                return null;
            }

            string pattern = "@[a-zA-Z0-9_]*";
            var matches = Regex.Matches(whereClause, pattern);


            var 值包裹 = new Type[] { typeof(string) };
            var 值包裹_日期 = new Type[] { typeof(DateTime), typeof(DateTime?) };
            //要倒序替换
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                var sqlParameterName = matches[i].Value;
                var sqlParameterValue = parameters[matches[i].Value];
                var sqlParameterValueType = sqlParameterValue.GetType();
                if (值包裹.Contains(sqlParameterValueType))
                {
                    whereClause = whereClause.Replace(sqlParameterName, $"'{sqlParameterValue}'");
                }
                else if (值包裹_日期.Contains(sqlParameterValueType))
                {
                    if (formatDateTime?.ContainsKey(sqlParameterName) == true)
                    {
                        whereClause = whereClause.Replace(sqlParameterName, $"'{((DateTime)sqlParameterValue).ToString(formatDateTime[sqlParameterName])}'");
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
            return whereClause;
        }

        /// <summary>
        /// sql参数合并到sql语句中
        /// </summary>
        /// <param name="sqlAndParam"></param>
        /// <param name="formatDateTime"></param>
        /// <returns></returns>
        public static string MergeParametersIntoSql(ValueTuple<string, Dictionary<string, object>> sqlAndParam,
             Dictionary<string, string> formatDateTime = null)
             => MergeParametersIntoSql(sqlAndParam.Item1, sqlAndParam.Item2, formatDateTime);

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


    }
}
