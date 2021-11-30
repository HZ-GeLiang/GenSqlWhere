using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToSqlWhereClause
{
    public static class ConditionBuilder
    {

        public static StringBuilder BuildCondition(MemberInfo memberInfo, WhereClauseAdhesive adhesive, ExpressionType comparison, object value)
        {
            string fieldName = adhesive.SqlAdapter.FormatColumnName(memberInfo);
            string parameterName = EnsureParameter(memberInfo, adhesive);
            adhesive.Parameters.Add($"@{parameterName}", value);
            var sb = new StringBuilder($"{fieldName} {ConditionBuilder.ToComparisonSymbol(comparison, memberInfo)} @{parameterName}");
            return sb;
        }

        /// <summary>
        /// 编译条件,获得where子句
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="adhesive"></param>
        /// <param name="comparison"></param>
        /// <returns>where子句</returns>
        public static StringBuilder BuildCondition(MemberInfo memberInfo, WhereClauseAdhesive adhesive, ExpressionType comparison)
        {
            string fieldName = adhesive.SqlAdapter.FormatColumnName(memberInfo);
            string parameterName = EnsureParameter(memberInfo, adhesive);
            StringBuilder sb = new StringBuilder();
#if DEBUG
            var clause = $"{fieldName} {ConditionBuilder.ToComparisonSymbol(comparison, memberInfo)} @{parameterName}";
            sb.Append(clause);
#else
            sb.Append(fieldName).Append(" ").Append(ConditionBuilder.ToComparisonSymbol(comparison, memberInfo)).Append(" @").Append(parameterName);
#endif
            return sb;

        }

        public static StringBuilder BuildLikeOrEqualCondition(MethodCallExpression methodCallExpression, WhereClauseAdhesive adhesive)
        {
            string symbol;
            string valueSymbol;
            switch (methodCallExpression.Method.Name)
            {
                case "Equals":
                    symbol = SqlKeys.Equals_symbol;
                    valueSymbol = SqlKeys.Equals_valueSymbol;
                    break;
                case "StartsWith":
                    symbol = SqlKeys.StartsWith_symbol;
                    valueSymbol = SqlKeys.StartsWith_valueSymbol;
                    break;
                case "EndsWith":
                    symbol = SqlKeys.EndsWith_symbol;
                    valueSymbol = SqlKeys.EndsWith_valueSymbol;
                    break;
                case "Contains":
                    symbol = SqlKeys.Contains_symbol;
                    valueSymbol = SqlKeys.Contains_valueSymbol;
                    break;
                default:
                    throw new NotSupportedException($"Not support method name:{methodCallExpression.Method.Name}");
            }

            if (methodCallExpression.Object is MemberExpression memberExpression)
            {
                var memberInfo = memberExpression.Member;
                string fieldName = adhesive.SqlAdapter.FormatColumnName(memberInfo);
                string parameterName = EnsureParameter(memberInfo, adhesive);
                object value = ConstantExtractor.ParseConstant(methodCallExpression.Arguments[0]);
                adhesive.Parameters.Add($"@{parameterName}", string.Format(valueSymbol, value));
                return new StringBuilder(string.Format($"{fieldName} {symbol}", $"@{parameterName}"));
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        public static StringBuilder BuildInCondition(MemberExpression memberExpression, Expression valueExpression, WhereClauseAdhesive adhesive)
        {
            var memberInfo = memberExpression.Member;
            string fieldName = adhesive.SqlAdapter.FormatColumnName(memberInfo);
            string parameterName = EnsureParameter(memberInfo, adhesive);
            object value = ConstantExtractor.ParseConstant(valueExpression);
            adhesive.Parameters.Add($"@{parameterName}", value);
            //return new StringBuilder($"{fieldName} {SqlKeys.@in} {parameterName}");
            var sb = new StringBuilder(32);
            sb.Append(fieldName).Append(" ").Append(SqlKeys.@in).Append(" @").Append(parameterName);
            return sb;
        }

        internal static string EnsureParameter(MemberInfo memberInfo, WhereClauseAdhesive adhesive)
        {
            string key = memberInfo.Name;
            int seed = 1;
            string tempKey = key;
            while (adhesive.Parameters.ContainsKey($"@{tempKey}"))
            {
                tempKey = key + seed;
                seed++;
            }
            return tempKey;
        }

        internal static string ToComparisonSymbol(ExpressionType expressionType, MemberInfo memberInfo)
        {
            if (expressionType == ExpressionType.Not)
            {
                //not 只对bool类型做支持
                if (memberInfo is PropertyInfo pi && pi.PropertyType == typeof(bool))
                {
                    return "!=";  //和 NotEqual 等价  , 
                }
            }
            return expressionType switch
            {
                ExpressionType.Equal => SqlKeys.Equal,
                ExpressionType.LessThan => SqlKeys.LessThan,
                ExpressionType.GreaterThan => SqlKeys.GreaterThan,
                ExpressionType.GreaterThanOrEqual => SqlKeys.GreaterThanOrEqual,
                ExpressionType.LessThanOrEqual => SqlKeys.LessThanOrEqual,
                ExpressionType.NotEqual => SqlKeys.NotEqual,
                //case ExpressionType.AndAlso:
                //    throw new Exceptions.EntityToSqlWhereCaluseConfigException("***");
                _ => throw new NotSupportedException($"Unknown ExpressionType {expressionType}")
            };
        }
    }
}
