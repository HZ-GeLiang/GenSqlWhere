using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_in_Attr
{
    [SearchType(SearchType.In)] public string Id { get; set; }
    [SearchType(SearchType.In)] public string Sex { get; set; }
}