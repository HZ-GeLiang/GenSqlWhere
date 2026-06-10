using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_timeRange2_Attr
{
    [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAt { get; set; }
}