using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Input_eq2
{
    [SearchType(SearchType.Eq)] public long? Id { get; set; }
}