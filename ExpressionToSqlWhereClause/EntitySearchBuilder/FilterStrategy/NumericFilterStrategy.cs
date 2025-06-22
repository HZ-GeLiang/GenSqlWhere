using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionToSqlWhereClause.EntitySearchBuilder
{
    /// <summary>
    /// 数值过滤策略实现
    /// </summary>
    /// <typeparam name="T">数据实体</typeparam>
    /// <typeparam name="TValue">数据实体的属性值</typeparam>
    public class NumericFilterStrategy<T, TValue> : IFilterStrategy<T, TValue>
            where T : class
        //where T : class where TValue : struct
    {
        private string _filterType;
        private TValue _value;

        public NumericFilterStrategy(TValue value, string filterType)
        {
            _filterType = filterType;
            _value = value;
        }

        public TValue Value { get => _value; set => _value = value; }
        public string FilterType { get => _filterType; set => _filterType = value; }

        //private bool IsNullableType(Type type) => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        //public static Type GetNullableTType(Type type) => type.GetProperty("Value").PropertyType;

        public Expression<Func<T, bool>> ApplyFilter(Expression<Func<T, TValue>> propertySelector)
        {
            if (_value == null)   //存在 where TValue : struct 时, 这里报错
            {
                return null;
            }

            //var isNullableType = IsNullableType(typeof(TValue));
            //if (isNullableType && object.Equals(_value, null))
            //{
            //    return null;
            //}

            var parameter = propertySelector.Parameters[0];
            var property = propertySelector.Body;
            //var constant = isNullableType
            //    ? Expression.Constant(_value, typeof(TValue))
            //    : Expression.Constant(_value);

            var constant = Expression.Constant(_value, typeof(TValue));//如果是nullable类型, 需要指定第二个参数

            Expression condition = null;
            switch (_filterType)
            {
                case "=":
                    condition = Expression.Equal(property, constant);
                    break;

                case "<>":
                case "!=":
                    condition = Expression.NotEqual(property, constant);
                    break;

                case ">":
                    condition = Expression.GreaterThan(property, constant);
                    break;

                case ">=":
                    condition = Expression.GreaterThanOrEqual(property, constant);
                    break;

                case "<":
                    condition = Expression.LessThan(property, constant);
                    break;

                case "<=":
                    condition = Expression.LessThanOrEqual(property, constant);
                    break;

                default:
                    break;
            }

            return condition != null
                ? Expression.Lambda<Func<T, bool>>(condition, parameter)
                : null;
        }
    }
}