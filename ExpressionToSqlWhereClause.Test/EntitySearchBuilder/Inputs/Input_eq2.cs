using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_eq2
{
    [SearchType(SearchType.Eq)] public long? Id { get; set; }
}