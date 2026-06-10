using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_timeRange_Attr
{
    [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAtStart { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAtEnd { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataUpdatedAtStart { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataUpdatedAtEnd { get; set; }
}