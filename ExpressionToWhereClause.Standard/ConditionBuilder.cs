﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToWhereClause.Standard
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
                    symbol = "= {0}";
                    valueSymbol = "{0}";
                    break;
                case "StartsWith":
                    symbol = "like {0}";
                    valueSymbol = "{0}%";
                    break;
                case "EndsWith":
                    symbol = "like {0}";
                    valueSymbol = "%{0}";
                    break;
                case "Contains":
                    symbol = "like {0}";
                    valueSymbol = "%{0}%";
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
            return new StringBuilder(string.Format("{0} in {1}", fieldName, $"@{parameterName}"));
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
                ExpressionType.Equal => "=",
                ExpressionType.LessThan => "<",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.NotEqual => "<>",
                //case ExpressionType.AndAlso:
                //    throw new Exception("***");

                _ => throw new NotSupportedException($"Unknown ExpressionType {expressionType}")
            };
        }
    }
}