using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.EntitySearchBuilder
{
    /// <summary>
    /// 字符串过滤策略实现
    /// </summary>
    /// <typeparam name="T">数据实体</typeparam>
    /// <typeparam name="TValue">数据实体的属性值</typeparam>
    public class StringFilterStrategy<T, TValue> : IFilterStrategy<T, string> where T : class/* where TValue : struct*/
    {
        private string _filterType;
        private string _value;

        public StringFilterStrategy(string value, string filterType)
        {
            _filterType = filterType;
            _value = value;
        }

        public string Value { get => _value; set => _value = value; }

        public string FilterType { get => _filterType; set => _filterType = value; }


        public Expression<Func<T, bool>> ApplyFilter(Expression<Func<T, string>> propertySelector)
        {
            if (_value == null)   // where TValue : struct 时, 这里报错
            {
                return null;
            }

            var parameter = propertySelector.Parameters[0];
            var property = propertySelector.Body;
            var constant = Expression.Constant(_value);

            Expression condition = null;

            switch (_filterType)
            {
                case "=":
                    condition = Expression.Equal(property, Expression.Constant(_value));
                    break;
                case "<>":
                case "!="://冗余
                    condition = Expression.NotEqual(property, Expression.Constant(_value));
                    break;
                case "starts_with":
                    condition = Expression.Call(property, "StartsWith", null, Expression.Constant(_value));
                    break;
                case "ends_with":
                    condition = Expression.Call(property, "EndsWith", null, Expression.Constant(_value));
                    break;
                case "contains":
                    condition = Expression.Call(property, "Contains", null, Expression.Constant(_value));
                    break;
                case "not_contains":
                    condition = Expression.Not(Expression.Call(property, "Contains", null, Expression.Constant(_value)));
                    break;
            }

            return condition != null
                ? Expression.Lambda<Func<T, bool>>(condition, parameter)
                : null;
        }


    }

}
