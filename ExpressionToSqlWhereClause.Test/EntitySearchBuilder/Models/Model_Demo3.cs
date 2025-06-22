using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Modlesl;

public class Model_Demo3
{
    [SearchType(SearchType.Eq)]
    public int Id { get; set; }
}