using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_le_Attr
{
    [SearchType(SearchType.Le)] public long? Id { get; set; }
    [SearchType(SearchType.Le)] public DateTime? DataCreatedAt { get; set; }
}