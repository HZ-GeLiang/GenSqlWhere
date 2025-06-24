using ExpressionToSqlWhereClause.Consts;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Helpers;
using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionToSqlWhereClause.ExpressionTree;

public static class ConditionBuilder
{
    public static SqlClauseParametersInfo BuildCondition(Expression expression, MemberInfo memberInfo, WhereClauseAdhesive adhesive, ExpressionType comparison, object value)
    {
        string fieldName = GetFieldName(expression, memberInfo);
        fieldName = adhesive.SqlAdapter.FormatColumnName(fieldName);
        string parameterName = EnsureParameter(fieldName, adhesive);
        var parametersKey = $"@{parameterName}";
        var param = adhesive.GetParameter(parametersKey);
        var symbol = ConditionBuilder.ToComparisonSymbol(comparison, memberInfo);
        param.Symbol = symbol;
        param.Field = fieldName;
        param.Value = value;
        param.SqlClause = $"{fieldName} {symbol} {parametersKey}";
        return param;
    }

    public static string GetFieldName(Expression expression, MemberInfo memberInfo)
    {
        if (memberInfo.DeclaringType.IsNullableType() &&
            "Value" == memberInfo.Name
            )//a.Flag.Value
        {
            if (expression.GetType().FullName == ExpressionFullNameSpaceConst.Property)
            {
                Expression exp = (Expression)((dynamic)expression).Expression;//a.Flag
                MemberInfo member = (MemberInfo)((dynamic)exp).Member;
                return GetFieldName(exp, member);
            }
        }
        return memberInfo.Name;
    }

    /// <summary>
    /// 编译条件
    /// </summary>
    /// <param name="expression"></param>
    /// <param name="memberInfo"></param>
    /// <param name="adhesive"></param>
    /// <param name="comparison"></param>
    /// <returns> </returns>
    public static SqlClauseParametersInfo BuildCondition(Expression expression, MemberInfo memberInfo, WhereClauseAdhesive adhesive, ExpressionType comparison)
    {
        var fieldName = GetFieldName(expression, memberInfo);
        fieldName = adhesive.SqlAdapter.FormatColumnName(fieldName);
        string parameterName = EnsureParameter(fieldName, adhesive);
        var symbol = ConditionBuilder.ToComparisonSymbol(comparison, memberInfo);
        var parametersKey = $"@{parameterName}";
        var param = adhesive.GetParameter(parametersKey);
        param.Symbol = symbol;
        param.Field = fieldName;
        param.SqlClause = $"{fieldName} {symbol} {parametersKey}";
        return param;
    }

    public static SqlClauseParametersInfo BuildLikeOrEqualCondition(MethodCallExpression methodCallExpression, WhereClauseAdhesive adhesive)
    {
        if (methodCallExpression.Object is MemberExpression memberExpression)
        {
            #region 获得值: symbol + valueSymbol

            string symbol;
            string valueSymbol;
            switch (methodCallExpression.Method.Name)
            {
                case "Equals":
                    symbol = SqlKeys.Equals_symbol;
                    valueSymbol = SqlKeys.Equals_valueSymbol;
                    break;

                case "StartsWith": //like 的3种/1
                    symbol = SqlKeys.StartsWith_symbol;
                    valueSymbol = SqlKeys.StartsWith_valueSymbol;
                    break;

                case "EndsWith"://like 的3种/2
                    symbol = SqlKeys.EndsWith_symbol;
                    valueSymbol = SqlKeys.EndsWith_valueSymbol;
                    break;

                case "Contains"://like 的3种/3
                    symbol = SqlKeys.Contains_symbol;
                    valueSymbol = SqlKeys.Contains_valueSymbol;
                    break;

                default:
                    throw new NotSupportedException($"Not support method name:{methodCallExpression.Method.Name}");
            }

            #endregion

            var memberInfo = memberExpression.Member;
            string fieldName = GetFieldName(memberExpression, memberInfo);
            fieldName = adhesive.SqlAdapter.FormatColumnName(fieldName);
            string parameterName = EnsureParameter(fieldName, adhesive);
            object value = ConstantExtractor.ParseConstant(methodCallExpression.Arguments[0]);
            var parametersKey = $"@{parameterName}";
            var param = adhesive.GetParameter(parametersKey);
            param.Value = string.Format(valueSymbol, value);
            param.Symbol = symbol;
            param.Field = fieldName;
            param.SqlClause = string.Format($"{fieldName} {symbol}", parametersKey);// string.Format( Name Like ){0} , Name1
            return param;
        }

        throw new NotSupportedException();
    }

    public static SqlClauseParametersInfo BuildInCondition(MemberExpression memberExpression, Expression valueExpression, WhereClauseAdhesive adhesive)
    {
        var memberInfo = memberExpression.Member;
        string fieldName = GetFieldName(memberExpression, memberInfo);
        fieldName = adhesive.SqlAdapter.FormatColumnName(fieldName);
        string parameterName = EnsureParameter(fieldName, adhesive);
        object value = ConstantExtractor.ParseConstant(valueExpression);
        var parametersKey = $"@{parameterName}";
        var symbol = SqlKeys.@in;
        var param = adhesive.GetParameter(parametersKey);
        param.Symbol = symbol;
        param.Field = fieldName;
        param.Value = value;

        List<SqlClauseListParametersItem> valueCollection = new List<SqlClauseListParametersItem>();

        var hasEach = ForeachHelper.Each(value, (itorValue) =>
        {
            //var parameterKey = $"{parameterName}_itor";
            var parameterKey = $"{parameterName}";
            var itor_parameterName = EnsureParameter(parameterKey, adhesive);
            var itor_parametersKey = $"@{itor_parameterName}";
            var itor_param = adhesive.GetParameter(itor_parametersKey);
            itor_param.Value = itorValue;
            valueCollection.Add(new SqlClauseListParametersItem(itor_param));
        });
        if (hasEach)
        {
            param.SqlClause = $"{fieldName} {symbol} ({valueCollection.AggregateToString(a => a.Key, ", ")})";
        }
        else
        {
            param.SqlClause = $"{fieldName} {symbol} ({parametersKey})";
        }

        return param;
    }

    /// <summary>
    /// 确保参数名称正确
    /// </summary>
    /// <param name="fieldName"></param>
    /// <param name="adhesive"></param>
    /// <returns></returns>
    internal static string EnsureParameter(string fieldName, WhereClauseAdhesive adhesive)
    {
        ushort seed = 1; //ushort.MaxValue   65535
        string tempKey = fieldName;
        while (adhesive.ContainsParameter($"@{tempKey}")) //遍历到变量名不重名为止
        {
            tempKey = fieldName + seed;
            seed++;
        }
        return tempKey;
    }

    /// <summary>
    /// 获得比较符号
    /// </summary>
    /// <param name="expressionType"></param>
    /// <param name="methodCallExpression"></param>
    /// <returns></returns>
    internal static string ToComparisonSymbol(ExpressionType expressionType, MethodCallExpression methodCallExpression)
    {
        if (expressionType == ExpressionType.Equal)
        {
            if (methodCallExpression.Type.IsObjectCollection())
            {
                return SqlKeys.@in;
            }
        }
        var symbol = ToComparisonSymbol(expressionType, methodCallExpression.Method);
        return symbol;
    }

    /// <summary>
    /// 获得比较符号
    /// </summary>
    /// <param name="expressionType"></param>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    internal static string ToComparisonSymbol(ExpressionType expressionType, MemberInfo memberInfo)
    {
        if (expressionType == ExpressionType.Not)
        {
            //not 只对bool类型做支持
            if (memberInfo is PropertyInfo pi && pi.PropertyType == typeof(bool))
            {
                //return "!=";  //和 NotEqual 等价  ,
                return SqlKeys.NotEqual;
            }
        }

        var symbol = expressionType switch
        {
            ExpressionType.Equal => SqlKeys.Equal,
            ExpressionType.LessThan => SqlKeys.LessThan,
            ExpressionType.GreaterThan => SqlKeys.GreaterThan,
            ExpressionType.GreaterThanOrEqual => SqlKeys.GreaterThanOrEqual,
            ExpressionType.LessThanOrEqual => SqlKeys.LessThanOrEqual,
            ExpressionType.NotEqual => SqlKeys.NotEqual,
            //case ExpressionType.AndAlso:
            //    throw new ExpressionToSqlWhereClauseException("***");
            _ => throw new NotSupportedException($"Unknown ExpressionType {expressionType}")
        };
        return symbol;
    }
}