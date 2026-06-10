using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClauseTest;

public class Input_likeLeft_Attr
{
    [SearchType(SearchType.LikeLeft)] public string Url { get; set; }
    [SearchType(SearchType.LikeLeft)] public string Data_Remark { get; set; }
}