using ExpressionToSqlWhereClause.EntitySearchBuilder;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.ExtensionMethods
{
    /// <summary>
    /// 用来生成具体的FilterStrategy
    /// </summary>
    public static class FilterStrategyExtensions
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <typeparam name="TValue">必须和实体属性的类型保持一致</typeparam>
        /// <param name="query"></param>
        /// <param name="value"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public static IFilterStrategy<T, TValue> NumericFilterStrategy<T, TValue>(
            this IQueryable<T> query, TValue value, string filterType) where T : class
            //this IQueryable<T> query, TValue value, string filterType) where T : class where TValue : struct
        {
            return new NumericFilterStrategy<T, TValue>(value, filterType);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T">数据实体</typeparam>
        /// <typeparam name="TValue">必须和实体属性的类型保持一致</typeparam>
        /// <param name="query"></param>
        /// <param name="value"></param>
        /// <param name="filterType"></param>
        /// <returns></returns>
        public static IFilterStrategy<T, TValue> NumericFilterStrategy<T, TValue>(
            this Expression<Func<T, bool>> query, TValue value, string filterType) where T : class
            //this IQueryable<T> query, TValue value, string filterType) where T : class where TValue : struct
        {
            return new NumericFilterStrategy<T, TValue>(value, filterType);
        }
    }
}