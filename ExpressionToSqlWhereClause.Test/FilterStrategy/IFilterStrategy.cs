using System.Linq.Expressions;
using System.Reflection;

namespace FilterStrategy
{
    /// <summary>
    /// 定义过滤策略接口
    /// </summary>
    /// <typeparam name="T">数据实体</typeparam>
    /// <typeparam name="TValue">数据实体的属性值</typeparam>
    public interface IFilterStrategy<T, TValue> where T : class
    {
        /// <summary>
        /// 查询的值
        /// </summary>
        public TValue Value { get; set; }

        /// <summary>
        /// 查询的条件
        /// </summary>
        public string FilterType { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="propertySelector">数据实体的属性对象</param>
        /// <returns></returns>
        Expression<Func<T, bool>> ApplyFilter(Expression<Func<T, TValue>> propertySelector);

        //Expression<Func<T, bool>> ApplyFilter(Expression<Func<T, TValue>> propertySelector, TValue value);
    }
}