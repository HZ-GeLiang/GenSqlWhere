using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.ExtensionMethods;

#region 操作Expression的扩展方法

/*
这2个类来自,然后进行了稍微调整了,增加了null值判断
https://docs.microsoft.com/zh-cn/archive/blogs/meek/linq-to-entities-combining-predicates

 Represents the parameter rebinder used for rebinding the parameters
 for the given expressions. This is part of the solution which solves
 the expression parameter problem when going to Entity Framework.
 For more information about this solution please refer to http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx.  (这个地址失效了, 估计就是微软的docs那个)
*/

internal static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> WhereIf<T>(this Expression<Func<T, bool>> first, bool condition, Expression<Func<T, bool>> second)
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
    public static Expression<Func<T, bool>> WhereIf<T>(this Expression<Func<T, bool>> exp, bool condition,
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

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
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

    public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> first, bool condition, Func<Expression<Func<T, bool>>> second)
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

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
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

    public static Expression<Func<T, bool>> OrIf<T>(this Expression<Func<T, bool>> first, bool condition, Expression<Func<T, bool>> second)
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