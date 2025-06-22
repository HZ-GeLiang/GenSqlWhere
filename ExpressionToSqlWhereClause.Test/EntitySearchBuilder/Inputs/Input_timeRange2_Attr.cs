using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_timeRange2_Attr
{
    [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAt { get; set; }
}