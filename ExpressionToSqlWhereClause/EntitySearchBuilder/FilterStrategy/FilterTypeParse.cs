namespace ExpressionToSqlWhereClause.EntitySearchBuilder
{
    public class FilterTypeParse
    {
        public static readonly Dictionary<string, FilterOperator> _map = new(StringComparer.OrdinalIgnoreCase)
        {
            ["="] = FilterOperator.Equal,
            ["=="] = FilterOperator.Equal,
            ["==="] = FilterOperator.Equal,
            ["eq"] = FilterOperator.Equal,

            ["!="] = FilterOperator.NotEqual,
            ["<>"] = FilterOperator.NotEqual,
            ["ne"] = FilterOperator.NotEqual,

            [">"] = FilterOperator.GreaterThan,
            ["gt"] = FilterOperator.GreaterThan,

            [">="] = FilterOperator.GreaterThanOrEqual,
            ["gte"] = FilterOperator.GreaterThanOrEqual,

            ["<"] = FilterOperator.LessThan,
            ["lt"] = FilterOperator.LessThan,

            ["<="] = FilterOperator.LessThanOrEqual,
            ["lte"] = FilterOperator.LessThanOrEqual,

            ["between"] = FilterOperator.Between
        };

        public static FilterOperator ParseFilterOperator(string filterType)
        {
            if (string.IsNullOrWhiteSpace(filterType))
            {
                throw new ArgumentException("FilterType 不能为空");
            }

            var key = filterType.Trim();

            if (_map.TryGetValue(key, out var op))
            {
                return op;
            }

            throw new NotSupportedException($"不支持的 FilterType: {filterType}");
        }
    }
}
