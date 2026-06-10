using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_lt_Attr
{
    [SearchType(SearchType.Lt)] public long? Id { get; set; }
    [SearchType(SearchType.Lt)] public DateTime? DataCreatedAt { get; set; }
}