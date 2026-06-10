using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_eq
{
    [SearchType(SearchType.Eq)] public bool IsDel { get; set; }
}