using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_Demo3_不同配置
{
    [SearchType(SearchType.Eq)]
    public int Id { get; set; }
}