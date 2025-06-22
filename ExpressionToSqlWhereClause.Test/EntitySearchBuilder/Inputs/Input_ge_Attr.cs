using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_ge_Attr
{
    [SearchType(SearchType.Ge)] public long? Id { get; set; }
    [SearchType(SearchType.Ge)] public DateTime? DataCreatedAt { get; set; }
}