using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_neq_Attr
{
    [SearchType(SearchType.Neq)]
    public bool IsDel { get; set; }
}