using System.Linq.Expressions;
using System;
using ExpressionToSqlWhereClause.ExtensionMethod;
using ExpressionToSqlWhereClause.Helper;
using Microsoft.Data.SqlClient;

namespace ExpressionToSqlWhereClause.SqlServer.ExtensionMehtods
{
    public static class SearchConditionExtension
    {
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