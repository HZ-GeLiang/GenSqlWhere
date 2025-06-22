using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_eq
{
    [SearchType(SearchType.Eq)] public bool IsDel { get; set; }
}