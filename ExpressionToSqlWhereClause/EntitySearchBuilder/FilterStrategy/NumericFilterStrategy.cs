using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.EntitySearchBuilder
{
    /// <summary>
    /// 数值过滤策略实现
    /// </summary>
    /// <typeparam name="TDbEntity">数据实体</typeparam>
    /// <typeparam name="TDbEntityValue">数据实体的属性值</typeparam>
    public class NumericFilterStrategy<TDbEntity, TDbEntityValue> : IFilterStrategy<TDbEntity, TDbEntityValue>
          where TDbEntity : class
        //where TDbEntityValue : struct
    {

        #region 接口属性

        /// <summary>
        /// 查询的值
        /// </summary>
        public TDbEntityValue Value { get; set; }

        /// <summary>
        /// 查询的条件
        /// </summary>
        public FilterOperator FilterType { get; set; }

        #endregion

        public NumericFilterStrategy(TDbEntityValue value, string filterType)
        {
            this.Value = value;
            this.FilterType = FilterTypeParse.ParseFilterOperator(filterType);
        }

        public Expression<Func<TDbEntity, bool>> ApplyFilter(Expression<Func<TDbEntity, TDbEntityValue>> propertySelector)
        {
            if (Value == null)   //存在 where TDbEntityValue : struct 时, 这里编译会报错
            {
                return null;
            }

            //var isNullableType = IsNullableType(typeof(TDbEntityValue));
            //if (isNullableType && object.Equals(_value, null))
            //{
            //    return null;
            //}

            var parameter = propertySelector.Parameters[0];
            var property = propertySelector.Body;
            //var constant = isNullableType
            //    ? Expression.Constant(this.Value, typeof(TDbEntityValue))
            //    : Expression.Constant(this.Value);

            var constant = Expression.Constant(this.Value, typeof(TDbEntityValue));//如果是nullable类型, 需要指定第二个参数

            Expression condition = null;
            switch (this.FilterType)
            {
                case FilterOperator.Equal:
                    condition = Expression.Equal(property, constant);
                    break;

                case FilterOperator.NotEqual:
                    condition = Expression.NotEqual(property, constant);
                    break;

                case FilterOperator.GreaterThan:
                    condition = Expression.GreaterThan(property, constant);
                    break;

                case FilterOperator.GreaterThanOrEqual:
                    condition = Expression.GreaterThanOrEqual(property, constant);
                    break;

                case FilterOperator.LessThan:
                    condition = Expression.LessThan(property, constant);
                    break;

                case FilterOperator.LessThanOrEqual:
                    condition = Expression.LessThanOrEqual(property, constant);
                    break;

                default:
                    break;
            }

            return condition != null
                ? Expression.Lambda<Func<TDbEntity, bool>>(condition, parameter)
                : null;
        }
    }
}