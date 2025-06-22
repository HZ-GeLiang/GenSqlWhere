using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Input_neq_Attr
{
    [SearchType(SearchType.Neq)]
    public bool IsDel { get; set; }
}