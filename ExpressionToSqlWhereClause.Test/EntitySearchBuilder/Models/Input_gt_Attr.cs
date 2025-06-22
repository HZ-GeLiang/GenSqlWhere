using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Input_gt_Attr
{
    [SearchType(SearchType.Gt)]
    public long? Id { get; set; }

    [SearchType(SearchType.Gt)]
    public DateTime? DataCreatedAt { get; set; }
}