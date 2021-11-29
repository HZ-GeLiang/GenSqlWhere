using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EntityToSqlWhereCaluseConfig.ExtensionMethod
{
    #region 操作Expression的扩展方法
    /*
    这2个类来自
    https://docs.microsoft.com/zh-cn/archive/blogs/meek/linq-to-entities-combining-predicates

     Represents the parameter rebinder used for rebinding the parameters
     for the given expressions. This is part of the solution which solves
     the expression parameter problem when going to Entity Framework.
     For more information about this solution please refer to http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx.  (这个地址失效了, 估计就是微软的docs那个)
    */

    internal static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
            => first.Compose(second, Expression.AndAlso);

        public static Expression<Func<T, bool>> AndIf<T>(this Expression<Func<T, bool>> first, bool condition, Func<Expression<Func<T, bool>>> second)
            => condition ? first.Compose(second(), Expression.AndAlso) : first;

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
            => first.Compose(second, Expression.OrElse);

        public static Expression<Func<T, bool>> OrIf<T>(this Expression<Func<T, bool>> first, bool condition, Expression<Func<T, bool>> second)
            => condition ? first.Compose(second, Expression.OrElse) : first;


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

        internal ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        internal static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map,
            Expression exp)
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
}