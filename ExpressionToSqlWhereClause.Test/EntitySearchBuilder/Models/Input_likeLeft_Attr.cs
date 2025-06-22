using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Input_likeLeft_Attr
{
    [SearchType(SearchType.LikeLeft)] public string Url { get; set; }
    [SearchType(SearchType.LikeLeft)] public string Data_Remark { get; set; }
}