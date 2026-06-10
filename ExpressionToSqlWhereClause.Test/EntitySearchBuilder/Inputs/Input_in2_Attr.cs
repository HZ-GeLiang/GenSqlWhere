using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_in2_Attr
{
    [SearchType(SearchType.In)] public int? Id { get; set; }
}