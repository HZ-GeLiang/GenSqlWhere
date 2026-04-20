using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.Exceptions;
using ExpressionToSqlWhereClause.ExpressionTree;
using ExpressionToSqlWhereClause.ExpressionTree.Adapter;
using ExpressionToSqlWhereClause.Helpers;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause;

#region 操作Expression的扩展方法

/*
这2个类来自,然后进行了稍微调整了,增加了null值判断
https://docs.microsoft.com/zh-cn/archive/blogs/meek/linq-to-entities-combining-predicates

 Represents the parameter rebinder used for rebinding the parameters
 for the given expressions. This is part of the solution which solves
 the expression parameter problem when going to Entity Framework.
 For more information about this solution please refer to http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx.  (这个地址失效了, 估计就是微软的docs那个)
*/

public static class ExpressionExtensions
{
    #region ToWhereClause

    /// <summary>
    /// 转换为Where子句
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression">表达式</param>
    /// <param name="aliasDict">别名</param>
    /// <param name="sqlAdapter">适配器</param>
    /// <returns></returns>
    public static SearchCondition ToWhereClause<T>(
        this Expression<Func<T, bool>> expression,
        Dictionary<string, string> aliasDict = null,
        ISqlAdapter sqlAdapter = default) where T : class
    {
        if (expression == null)
        {
            return new SearchCondition();
        }

        #region 处理 alias

        aliasDict ??= new Dictionary<string, string>();

        foreach (var propertyInfo in ReflectionHelper.GetProperties<T>())
        {
            //优先级: 方法参数的 alias > Column
            if (aliasDict.ContainsKey(propertyInfo.Name))
            {
                continue;
            }
            var attrs = ReflectionHelper.GetAttributeForProperty<ColumnAttribute>(propertyInfo, true);
            if (attrs.Length == 1 && string.IsNullOrWhiteSpace(attrs[0].Name) == false) // attrs[0].Name 不为空
            {
                aliasDict[propertyInfo.Name] = attrs[0].Name;
            }
        }

        #endregion

        //m,获得parse结果
        var parseResult = WhereClauseParser.Parse(expression.Body, aliasDict, sqlAdapter);

        #region 处理 parseResult 的  Adhesive  和 WhereClause

        var parameterKeys = parseResult.Adhesive.GetParameterKeys();
        foreach (var key in parameterKeys)
        {
            var para = parseResult.Adhesive.GetParameter(key);
            var val = para.Value;

            // 处理 val == null 的情况
            if (val == null)
            {
                if (para.Symbol == SqlKeys.Equal || para.Symbol == SqlKeys.NotEqual)
                {
                    //为了支持 a.url == null , 此处需要翻译为 url is null
                    var sqlClauseOrigin = para.SqlClause;
                    var reg = "[ ].*[ ]@.*\\)$";
                    var strList = RegexHelper.GetStringByMatches(sqlClauseOrigin, reg);
                    if (strList.Count == 1)
                    {
                        string sqlClauseNew;
                        if (para.Symbol == SqlKeys.Equal)
                        {
                            sqlClauseNew = sqlClauseOrigin.Replace(strList[0], " Is Null)");
                        }
                        else if (para.Symbol == SqlKeys.NotEqual)
                        {
                            sqlClauseNew = sqlClauseOrigin.Replace(strList[0], " Is Not Null)");
                        }
                        else
                        {
                            throw new FrameException("逻辑错误,需要调整");
                        }

                        parseResult.WhereClause = parseResult.WhereClause.Replace(sqlClauseOrigin, sqlClauseNew);
                        parseResult.Adhesive.RemoveParameter(key);//移除参数

                        continue;
                    }
                    //else 没有处理
                }

                //else 没有处理
            }

            //翻译IEnumerable的值
            var parseValue = ConstantExtractor.ConstantExpressionValueToString(val, out var isIEnumerableObj, out var hasNullItem);
            if (isIEnumerableObj)
            {
                para.Value = parseValue;
                if (hasNullItem)
                {
                    para.SqlClause += $" {SqlKeys.LogicSymbolOr} ({para.Field} IS NULL)";
                    parseResult.WhereClause = para.SqlClause;
                }
                continue;
            }
        }

        #endregion

        var result = new SearchCondition(parseResult);
        return result;
    }

    #endregion

    #region 拼接Expression

    internal static Expression<Func<T, bool>> WhereIf<T>(this Expression<Func<T, bool>> first, bool condition, Expression<Func<T, bool>> second)
    {
        if (condition)
        {
            if (first == null)
            {
                if (second == null)
                {
                    return first;
                }
                else
                {
                    return second;
                }
            }
            else
            {
                return first.And(second);
            }
        }
        else
        {
            return first;
        }
    }

    /// <summary>
    /// WhereIf的 if - else  版本
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="exp"></param>
    /// <param name="condition"></param>
    /// <param name="predicate_conditionTrue"></param>
    /// <param name="predicate_conditionFalse"></param>
    /// <returns></returns>
    internal static Expression<Func<T, bool>> WhereIf<T>(this Expression<Func<T, bool>> exp, bool condition,
                            Expression<Func<T, bool>> predicate_conditionTrue,
                            Expression<Func<T, bool>> predicate_conditionFalse)
    {
        if (exp == null)
        {
            if (condition)
            {
                return predicate_conditionTrue;
            }
            else
            {
                return predicate_conditionFalse;
            }
        }
        else
        {
            if (condition)
            {
                return exp.And(predicate_conditionTrue);
            }
            else
            {
                return exp.And(predicate_conditionFalse);
            }
        }
    }

    internal static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        if (first == null)
        {
            if (second == null)
            {
                return first;
            }
            else
            {
                return second;
            }
        }
        else
        {
            return first.Compose(second, Expression.AndAlso);
        }
    }

    internal static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> first, bool condition, Func<Expression<Func<T, bool>>> second)
    {
        if (condition)
        {
            if (first == null)
            {
                if (second == null)
                {
                    return first;
                }
                else
                {
                    return second();
                }
            }
            else
            {
                return first.Compose(second(), Expression.AndAlso);
            }
        }
        else
        {
            return first;
        }
    }

    internal static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
    {
        if (first == null)
        {
            if (second == null)
            {
                return first;
            }
            else
            {
                return second;
            }
        }
        else
        {
            return first.Compose(second, Expression.OrElse);
        }
    }

    internal static Expression<Func<T, bool>> OrIf<T>(this Expression<Func<T, bool>> first, bool condition, Expression<Func<T, bool>> second)
    {
        if (condition)
        {
            if (first == null)
            {
                if (second == null)
                {
                    return first;
                }
                else
                {
                    return second;
                }
            }
            else
            {
                return first.Compose(second, Expression.OrElse);
            }
        }
        else
        {
            return first;
        }
    }

    private static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
    {
        // build parameter map(from parameters of second to parameters of first)
        var map = first.Parameters
            .Select((f, i) => new
            {
                f,
                s = second.Parameters[i]
            })
            .ToDictionary(p => p.s, p => p.f);

        // replace parameters in the second lambda expression with parameters from the first
        var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

        // apply composition of lambda expression bodies to parameters from the first expression
        var body = merge(first.Body, secondBody);
        return Expression.Lambda<T>(body, first.Parameters);
    }

    #endregion

    #region 二次封装具体的FilterStrategy

    /// <summary>
    /// 自动判断：数值不为null + 过滤字符串不为null
    /// 自动拼接：NumericFilterStrategy
    /// </summary>
    public static Expression<Func<T, bool>> WhereIfNumericFilterStrategy<T, TValue>(
        this Expression<Func<T, bool>> expression,
        string filterFilter,          // valueFilter (string)
        TValue? filterValue,          // value (可空数值)
        Expression<Func<T, TValue>> propertySelector) // a => a.GetSum
        where T : class
        where TValue : struct, IComparable
    {
        if (filterValue.HasValue && !string.IsNullOrWhiteSpace(filterFilter))
        {
            expression = expression.WhereIfFilterStrategy(
                condition: true,
                propertySelector,
                strategy: b => b.NumericFilterStrategy(filterValue.Value, filterFilter)
            );
        }

        return expression;
    }


    /// <summary>
    /// 智能日期过滤（自动区分：单日期 / 双日期范围）
    /// 严格匹配业务规则：
    /// 1. 单日期：开始日期+过滤类型有值，结束日期 = null
    /// 2. 双日期：开始日期+过滤类型有值，结束日期 有值
    /// 3. 不满足则不拼接条件
    /// </summary>
    public static Expression<Func<T, bool>> WhereIfDateTimeFilterStrategy<T, TValue>(
        this Expression<Func<T, bool>> expression,
        string filterType,
        TValue? startDate,
        TValue? endDate,
        Expression<Func<T, TValue>> propertySelector)
        where T : class
        where TValue : struct, IComparable
    {
        if (startDate.HasValue && !string.IsNullOrWhiteSpace(filterType))
        {
            if (endDate.HasValue)// 双日期
            {
                expression = expression.WhereIfFilterStrategy(
                   condition: true,
                   propertySelector,
                   strategy: b => b.DateTimeFilterStrategy(startDate.Value, filterType, endDate.Value)
                );

            }
            else
            {
                expression = expression.WhereIfFilterStrategy(
                    condition: true,
                    propertySelector,
                    strategy: b => b.DateTimeFilterStrategy(startDate.Value, filterType)
                );
            }
        }
        return expression;
    }

    #endregion

    #region 常规封装


    /// <summary>
    /// 追加时间范围的条件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="query"></param>
    /// <param name="fieldSelector">字段选择：a => a.CreateDate</param>
    /// <param name="startTime">开始日期(含)</param>
    /// <param name="endTime">结束日期(含)</param>
    /// <param name="isFieldDateWithTime">数据库字段值含有时分秒信息</param>
    /// <returns></returns>
    public static IQueryable<T> WhereIfBetweenDates<T>(
        this IQueryable<T> query,
        Expression<Func<T, DateTime?>> fieldSelector,
        DateTime? startTime,
        DateTime? endTime,
        bool isFieldDateWithTime)
    {
        var predicate = WhereIfBetweenDates(fieldSelector, startTime, endTime, isFieldDateWithTime);
        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return query;
    }


    /// <summary>
    /// 追加时间范围的条件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="expression"></param>
    /// <param name="fieldSelector">字段选择：a => a.CreateDate</param>
    /// <param name="startTime">开始日期(含)</param>
    /// <param name="endTime">结束日期(含)</param>
    /// <param name="isFieldDateWithTime">数据库字段值含有时分秒信息</param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> WhereIfBetweenDates<T>(
        this Expression<Func<T, bool>> expression,
        Expression<Func<T, DateTime?>> fieldSelector,
        DateTime? startTime,
        DateTime? endTime,
        bool isFieldDateWithTime)
    {
        var exp = WhereIfBetweenDates(fieldSelector, startTime, endTime, isFieldDateWithTime);
        if (exp != null)
        {
            expression = expression.WhereIf(true, exp);
        }
        return expression;
    }

    /// <summary>
    /// 构建时间范围表达式
    /// </summary>
    /// <typeparam name="T">实体类</typeparam>
    /// <param name="fieldSelector">字段选择：a => a.CreateDate</param>
    /// <param name="startTime">开始日期(含)</param>
    /// <param name="endTime">结束日期(含)</param>
    /// <param name="isFieldDateWithTime">数据库字段值含有时分秒信息:若值true, endTime 的值为 yyyy-MM-dd 时，结束日期自动增加1天</param>
    /// <returns>组合好的表达式</returns>
    public static Expression<Func<T, bool>> WhereIfBetweenDates<T>(
        Expression<Func<T, DateTime?>> fieldSelector,
        DateTime? startTime,
        DateTime? endTime,
        bool isFieldDateWithTime)
    {
        var parameter = fieldSelector.Parameters[0];
        var member = fieldSelector.Body;
        Expression? exp = null;

        if (startTime.HasValue)
        {
            //var value = DateTime.SpecifyKind(startTime.Value, DateTimeKind.Unspecified);
            var value = startTime.Value;
            var startValue = Expression.Constant(value, typeof(DateTime?));
            exp = Expression.GreaterThanOrEqual(member, startValue);
        }

        if (endTime.HasValue)
        {
            //数据库字段值含有时分秒信息 + 当结束日期(endTime)只有年月日 , 自动给结束日期 + 1 天
            if (isFieldDateWithTime && endTime.Value.TimeOfDay == TimeSpan.Zero)
            {
                endTime = endTime.Value.AddDays(1);
            }

            //var value = DateTime.SpecifyKind(endTime.Value, DateTimeKind.Unspecified);
            var value = endTime.Value;
            var endValue = Expression.Constant(value, typeof(DateTime?));
            if (exp == null)
            {
                exp = Expression.LessThanOrEqual(member, endValue);
            }
            else
            {
                exp = Expression.AndAlso(exp, Expression.LessThanOrEqual(member, endValue));
            }
        }

        if (exp != null)
        {
            return Expression.Lambda<Func<T, bool>>(exp, parameter);
        }

        return null;
    }

    #endregion
}

internal class ParameterRebinder : ExpressionVisitor
{
    private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

    public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
    {
        this._map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
    }

    public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
    {
        return new ParameterRebinder(map).Visit(exp);
    }

    protected override Expression VisitParameter(ParameterExpression p)
    {
        if (_map.TryGetValue(p, out var replacement))
        {
            p = replacement;
        }
        return base.VisitParameter(p);
    }
}

#endregion