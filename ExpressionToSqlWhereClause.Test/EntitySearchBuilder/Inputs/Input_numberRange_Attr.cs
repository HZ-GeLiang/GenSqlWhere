using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_numberRange_Attr
{
    [SearchType(SearchType.NumberRange)] public int? IdLeft { get; set; }
    [SearchType(SearchType.NumberRange)] public int? IdRight { get; set; }
}