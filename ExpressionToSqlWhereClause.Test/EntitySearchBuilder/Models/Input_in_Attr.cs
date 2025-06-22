using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Input_in_Attr
{
    [SearchType(SearchType.In)] public string Id { get; set; }
    [SearchType(SearchType.In)] public string Sex { get; set; }
}