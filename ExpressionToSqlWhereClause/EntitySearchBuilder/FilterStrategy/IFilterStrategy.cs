using System.Linq.Expressions;
using System.Reflection;

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
        public string FilterType { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertySelector">数据实体的属性对象</param>
        /// <returns></returns>
        Expression<Func<TDbEntity, bool>> ApplyFilter(Expression<Func<TDbEntity, TDbEntityValue>> propertySelector);

        //Expression<Func<T, bool>> ApplyFilter(Expression<Func<T, TValue>> propertySelector, TValue value);
    }
}