using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_le_Attr
{
    [SearchType(SearchType.Le)] public long? Id { get; set; }
    [SearchType(SearchType.Le)] public DateTime? DataCreatedAt { get; set; }
}