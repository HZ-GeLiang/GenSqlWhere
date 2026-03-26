using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.EntitySearchBuilder
{
    public class DateTimeFilterStrategy<TDbEntity, TDbEntityValue>
      : IFilterStrategy<TDbEntity, TDbEntityValue>
      where TDbEntity : class
    {
        public TDbEntityValue Value { get; set; }
        public TDbEntityValue Value2 { get; set; }
        public FilterOperator FilterType { get; set; }

        public DateTimeFilterStrategy(TDbEntityValue value, string filterType, TDbEntityValue value2 = default)
        {
            Value = value;
            Value2 = value2;
            FilterType = FilterTypeParse.ParseFilterOperator(filterType);
            if (FilterType == FilterOperator.Between && EqualityComparer<TDbEntityValue>.Default.Equals(Value, default))
            {
                throw new ArgumentNullException($@"DateTimeFilterStrategy的filterType为Between时, 参数2不能为空");
            }
        }

        public Expression<Func<TDbEntity, bool>> ApplyFilter(
            Expression<Func<TDbEntity, TDbEntityValue>> propertySelector)
        {
            // 通用判断
            if (FilterType != FilterOperator.Between &&
                EqualityComparer<TDbEntityValue>.Default.Equals(Value, default))
            {
                return null;
            }

            var parameter = propertySelector.Parameters[0];
            var property = propertySelector.Body;

            var constant1 = Expression.Constant(Value, typeof(TDbEntityValue));

            Expression condition = null;

            switch (this.FilterType)
            {
                case FilterOperator.Equal:
                    condition = Expression.Equal(property, constant1);
                    break;

                case FilterOperator.NotEqual:
                    condition = Expression.NotEqual(property, constant1);
                    break;

                case FilterOperator.GreaterThan:
                    condition = Expression.GreaterThan(property, constant1);
                    break;

                case FilterOperator.GreaterThanOrEqual:
                    condition = Expression.GreaterThanOrEqual(property, constant1);
                    break;

                case FilterOperator.LessThan:
                    condition = Expression.LessThan(property, constant1);
                    break;

                case FilterOperator.LessThanOrEqual:
                    condition = Expression.LessThanOrEqual(property, constant1);
                    break;

                case FilterOperator.Between:
                    var constant2 = Expression.Constant(Value2, typeof(TDbEntityValue));
                    var ge = Expression.GreaterThanOrEqual(property, constant1);
                    var le = Expression.LessThanOrEqual(property, constant2);
                    condition = Expression.AndAlso(ge, le);
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