using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;


namespace EntityToSqlWhereClauseConfig.Helper
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
        /// <returns></returns>
        public static string MergeParametersIntoSql(string whereClause, Dictionary<string, object> parameters)
        {
            if (string.IsNullOrWhiteSpace(whereClause))
            {
                return null;
            }

            string pattern = "@[a-zA-Z0-9_]*";
            var matches = Regex.Matches(whereClause, pattern);

            var typeString = typeof(string);
            //要倒序替换
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                var sqlParameterName = matches[i].Value;
                var sqlParameterValue = parameters[matches[i].Value];
                if (sqlParameterValue.GetType() == typeString)
                {
                    whereClause = whereClause.Replace(sqlParameterName, $"'{sqlParameterValue}'");
                }
                else
                {
                    whereClause = whereClause.Replace(sqlParameterName, $"{sqlParameterValue}");
                }
            }
            return whereClause;
        }

        public static string MergeParametersIntoSql(ValueTuple<string, Dictionary<string, object>> sqlAndParam)
            => MergeParametersIntoSql(sqlAndParam.Item1, sqlAndParam.Item2);



    }
}
