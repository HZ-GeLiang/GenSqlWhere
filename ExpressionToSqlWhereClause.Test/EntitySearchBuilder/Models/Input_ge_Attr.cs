using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Input_ge_Attr
{
    [SearchType(SearchType.Ge)] public long? Id { get; set; }
    [SearchType(SearchType.Ge)] public DateTime? DataCreatedAt { get; set; }
}