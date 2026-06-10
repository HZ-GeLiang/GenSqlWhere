using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_eq2
{
    [SearchType(SearchType.Eq)] public long? Id { get; set; }
}