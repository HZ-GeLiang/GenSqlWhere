#pragma warning disable IDE0130

using System.Linq.Expressions;

namespace Infra.ExtensionMethods
{
    //参考 PredicateBuilder 改造的
    public static class ExpressionExtensions_FuncBuilder
    {
        public static Expression<Func<T, T>> GetDefault<T>(Func<T, T> func)
        { return a => func.Invoke(a); }

        public static Expression<Func<T, T>> And<T>(Expression<Func<T, T>> first, Expression<Func<T, T>> second)
        {
            return first.AndAlso(second, Expression.AndAlso);
        }

        public static Expression<Func<T, T>> Or<T>(Expression<Func<T, T>> first, Expression<Func<T, T>> second)
        {
            return first.AndAlso(second, Expression.OrElse);
        }

        private static Expression<Func<T, T>> AndAlso<T>(this Expression<Func<T, T>> expr1, Expression<Func<T, T>> expr2, Func<Expression, Expression, BinaryExpression> func)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, T>>(
                func(left, right), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }
    }
}

#pragma warning restore IDE0130