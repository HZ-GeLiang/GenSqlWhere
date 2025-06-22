using ExpressionToSqlWhereClause.EntitySearchBuilder;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.ExtensionMethods
{
    /// <summary>
    /// 用来拼接具体的where
    /// </summary>
    public static class FilterStrategyQueryableExtensions
    {
        #region IQueryable

        public static IQueryable<T> WhereIfFilterStrategy<T, TValue>(
           this IQueryable<T> query,
           bool condition,
           Expression<Func<T, TValue>> propertySelector,
           IFilterStrategy<T, TValue> strategy) where T : class
        {
            /* 使用示例
                var query = _Repository.GetAll().Where()....;

                query=query.WhereIfFilterStrategy(obj.FinancePrice.HasValue() && obj.FinancePriceFilter.HasValue(),
                                            a => a.FinancePrice, query.NumericFilterStrategy(obj.FinancePrice.Value, obj.FinancePriceFilter));
            */
            if (condition == false)
            {
                return query;
            }

            return WhereFilterStrategy(query, propertySelector, strategy);
        }

        public static IQueryable<T> WhereIfFilterStrategy<T, TValue>(
           this IQueryable<T> query,
           bool condition,
           Expression<Func<T, TValue>> propertySelector,
           Func<IQueryable<T>, IFilterStrategy<T, TValue>> strategy) where T : class
        {
            /* 使用示例
                _Repository.GetAll()
                .Where()
                ...
                .WhereIfFilterStrategy(obj.FinancePrice.HasValue() && obj.FinancePriceFilter.HasValue(),
                                            a => a.FinancePrice, b => b.NumericFilterStrategy(obj.FinancePrice.Value, obj.FinancePriceFilter));
            */
            if (condition == false)
            {
                return query;
            }

            return WhereFilterStrategy(query, propertySelector, strategy.Invoke(query));
        }

        public static IQueryable<T> WhereFilterStrategy<T, TValue>(
            this IQueryable<T> query,
            Expression<Func<T, TValue>> propertySelector,
            IFilterStrategy<T, TValue> strategy) where T : class
        {
            /* 使用示例
               var query = _Repository.GetAll().Where()....;

               if(obj.FinancePrice.HasValue() && obj.FinancePriceFilter.HasValue())
                    query=query.WhereIfFilterStrategy(a => a.FinancePrice, query.NumericFilterStrategy(obj.FinancePrice.Value, obj.FinancePriceFilter));
           */

            if (strategy == null || strategy.Value == null)
            {
                return query;
            }

            var filterExpression = strategy.ApplyFilter(propertySelector);
            return filterExpression != null ? query.Where(filterExpression) : query;
        }

        #endregion

        #region Expression

        public static Expression<Func<T, bool>> WhereIfFilterStrategy<T, TValue>(
            this Expression<Func<T, bool>> expression,
            bool condition,
            Expression<Func<T, TValue>> propertySelector,
            IFilterStrategy<T, TValue> strategy) where T : class
        {
            if (condition == false)
            {
                return expression;
            }

            return WhereFilterStrategy(expression, propertySelector, strategy);
        }

        public static Expression<Func<T, bool>> WhereIfFilterStrategy<T, TValue>(
           this Expression<Func<T, bool>> expression,
           bool condition,
           Expression<Func<T, TValue>> propertySelector,
           Func<Expression<Func<T, bool>>, IFilterStrategy<T, TValue>> strategy) where T : class
        {
            if (condition == false)
            {
                return expression;
            }

            return WhereFilterStrategy(expression, propertySelector, strategy.Invoke(expression));
        }

        public static Expression<Func<T, bool>> WhereFilterStrategy<T, TValue>(
            this Expression<Func<T, bool>> expression,
            Expression<Func<T, TValue>> propertySelector,
            IFilterStrategy<T, TValue> strategy) where T : class
        {
            if (strategy == null || strategy.Value == null)
            {
                return expression;
            }

            var filterExpression = strategy.ApplyFilter(propertySelector);
            return filterExpression != null ? expression.And(filterExpression) : expression;
        }

        #endregion
    }
}