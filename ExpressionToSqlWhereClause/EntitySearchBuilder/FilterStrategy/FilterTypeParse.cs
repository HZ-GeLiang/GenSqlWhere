using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSqlWhereClause.EntitySearchBuilder
{
    internal class FilterTypeParse
    {
        public static FilterOperator ParseFilterOperator(string filterType)
        {
            if (string.IsNullOrWhiteSpace(filterType))
            {
                throw new ArgumentException("FilterType 不能为空");
            }

            switch (filterType.Trim().ToLower())
            {
                case "=":
                case "==":
                case "eq":
                    return FilterOperator.Equal;

                case "!=":
                case "<>":
                case "ne":
                    return FilterOperator.NotEqual;

                case ">":
                case "gt":
                    return FilterOperator.GreaterThan;

                case ">=":
                case "gte":
                    return FilterOperator.GreaterThanOrEqual;

                case "<":
                case "lt":
                    return FilterOperator.LessThan;

                case "<=":
                case "lte":
                    return FilterOperator.LessThanOrEqual;

                //case "between":
                //    return FilterOperator.Between;

                //case "date":
                //    return FilterOperator.Date;

                default:
                    throw new NotSupportedException($"不支持的 FilterType: {filterType}");
            }
        }
    }
}
