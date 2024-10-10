using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Helpers;
using Microsoft.Data.SqlClient;

namespace ExpressionToSqlWhereClause.SqlServer.ExtensionMehtods
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class SearchConditionExtensions
    {
        /// <summary>
        /// 获得sql参数
        /// </summary>
        /// <param name="SearchCondition"></param>
        /// <returns></returns>
        public static SqlParameter[] GetSqlParameter(this SearchCondition SearchCondition)
        {
            SqlParameter[] pms = WhereClauseHelper.ConvertParameters(SearchCondition.Parameters, (key, val) =>
            {
                return new SqlParameter { ParameterName = key, Value = val };
            });

            return pms;
        }
    }
}