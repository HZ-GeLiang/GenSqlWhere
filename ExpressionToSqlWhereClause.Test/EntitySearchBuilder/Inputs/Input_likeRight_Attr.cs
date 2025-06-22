using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_likeRight_Attr
{
    [SearchType(SearchType.LikeRight)] public string Url { get; set; }
    [SearchType(SearchType.LikeRight)] public string Data_Remark { get; set; }
}