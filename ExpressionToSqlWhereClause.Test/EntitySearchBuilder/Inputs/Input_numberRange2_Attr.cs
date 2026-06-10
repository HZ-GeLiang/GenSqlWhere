using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_numberRange2_Attr
{
    [SearchType(SearchType.NumberRange)] public int? Id { get; set; }
}