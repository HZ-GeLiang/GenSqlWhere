using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Input_eq
{
    [SearchType(SearchType.Eq)] public bool IsDel { get; set; }
}