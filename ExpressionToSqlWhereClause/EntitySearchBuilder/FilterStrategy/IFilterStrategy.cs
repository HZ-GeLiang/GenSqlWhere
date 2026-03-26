using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.EntitySearchBuilder
{
    /// <summary>
    /// 定义过滤策略接口
    /// </summary>
    /// <typeparam name="TDbEntity">数据实体</typeparam>
    /// <typeparam name="TDbEntityValue">数据实体的属性值</typeparam>
    public interface IFilterStrategy<TDbEntity, TDbEntityValue> where TDbEntity : class
    {
        /// <summary>
        /// 查询的值
        /// </summary>
        public TDbEntityValue Value { get; set; }

        /// <summary>
        /// 查询的条件
        /// </summary>
        public FilterOperator FilterType { get; set; }

        /// <summary>
        /// 应用过滤
        /// </summary>
        /// <param name="propertySelector">实体属性选择表达式</param>
        /// <returns>过滤表达式</returns>
        Expression<Func<TDbEntity, bool>> ApplyFilter(Expression<Func<TDbEntity, TDbEntityValue>> propertySelector);

        //Expression<Func<T, bool>> ApplyFilter(Expression<Func<T, TValue>> propertySelector, TValue value);
    }
}