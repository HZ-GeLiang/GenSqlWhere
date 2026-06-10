using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_neq_Attr
{
    [SearchType(SearchType.Neq)]
    public bool IsDel { get; set; }
}