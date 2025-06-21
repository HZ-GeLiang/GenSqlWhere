#pragma warning disable IDE0130

using ExpressionToSqlWhereClause.Helpers;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.ExtensionMethods
{
    public static class ExpressionExtensions_WhereValue
    {
        public static Expression<Func<T, bool>> WhereHasValue<T>(this T _, Expression<Func<T, string>> expression) where T : class
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return WhereLambdaHelper.GetExpression_HasValue<T>(memberExpression.Member.Name);
            }

            throw new ArgumentException("Invalid expression. Expected MemberExpression.");
        }

        public static Expression<Func<T, bool>> WhereNoValue<T>(this T _, Expression<Func<T, string>> expression) where T : class
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                return WhereLambdaHelper.GetExpression_NoValue<T>(memberExpression.Member.Name);
            }
            throw new ArgumentException("Invalid expression. Expected MemberExpression.");
        }
    }

    /*
     *
     * gpt 给的方式
    public static class ExpressionExtensions2
    {
        public static Expression<Func<TEntity, bool>> WhereHasValue<TEntity>(this Expression<Func<TEntity, string>> expression)
        {
            var parameter = expression.Parameters[0];
            var body = expression.Body;

            // 创建一个新的表达式，检查属性值是否为非空字符串
            var nullOrEmptyExpression = Expression.NotEqual(body, Expression.Constant(string.Empty));

            // 创建一个Lambda表达式，将参数传递给新的表达式
            var lambdaExpression = Expression.Lambda<Func<TEntity, bool>>(nullOrEmptyExpression, parameter);

            return lambdaExpression;
        }
    }

    public class StringWrapper<TEntity>
    {
        private Expression<Func<TEntity, string>> expression;

        public StringWrapper(Expression<Func<TEntity, string>> expression)
        {
            this.expression = expression;
        }

        public Expression<Func<TEntity, bool>> WhereHasValue()
        {
            return expression.WhereHasValue();
        }
    }*/
}

#pragma warning restore IDE0130