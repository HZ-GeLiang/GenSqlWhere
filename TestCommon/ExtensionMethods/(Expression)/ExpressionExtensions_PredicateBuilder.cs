#pragma warning disable IDE0130

using System.Linq.Expressions;

namespace Infra.ExtensionMethods
{
    //https://blog.csdn.net/weixin_47492910/article/details/116020315

    /*
        特别注意 ：拼接条件时，所使用到的条件 strID，strBir 必须是独立的
        var predicate = PredicateBuilder.GetTrue<Student>();
        predicate = predicate.And(it => it.id.ToString().Contains(strID));
        predicate = predicate.And(it => it.Birthday.ToString().Contains(strBir));
        predicate = predicate.And(it => it.Sex.ToString().Contains(strSex));
        predicate = predicate.And(it => it.Age == 20);
        var lst = db.Queryable<Student>.Where(predicate).ToListDataRow();
    */

    public static class ExpressionExtensions_PredicateBuilder
    {
        public static Expression<Func<T, bool>> GetTrue<T>()
        { return f => true; }

        public static Expression<Func<T, bool>> GetFalse<T>()
        { return f => false; }

        public static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.AndAlso(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.AndAlso(second, Expression.OrElse);
        }

        private static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2, Func<Expression, Expression, BinaryExpression> func)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(
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