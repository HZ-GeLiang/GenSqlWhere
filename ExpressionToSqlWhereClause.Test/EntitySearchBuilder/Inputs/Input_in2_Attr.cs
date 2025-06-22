using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_in2_Attr
{
    [SearchType(SearchType.In)] public int? Id { get; set; }
}