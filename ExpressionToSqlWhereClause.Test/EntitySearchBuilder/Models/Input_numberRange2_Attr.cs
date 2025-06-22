using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Input_numberRange2_Attr
{
    [SearchType(SearchType.NumberRange)] public int? Id { get; set; }
}