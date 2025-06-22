using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Input_in2_Attr
{
    [SearchType(SearchType.In)] public int? Id { get; set; }
}