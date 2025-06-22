using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

//demo2,  在 input 模型上 标记 Attribute 的这种配置方式来创建sql (推荐)
//注: demo1 和demo2 如果写重复, 那么都会生效
public class Input_Demo_Attr
{
    [SearchType(SearchType.In)] public string Id { get; set; }
    [SearchType(SearchType.Like)] public string Url { get; set; }
    [SearchType(SearchType.In)] public string Sex { get; set; }
    [SearchType(SearchType.Eq)] public bool IsDel { get; set; }
    [SearchType(SearchType.Like)] public string Data_Remark { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAtStart { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataCreatedAtEnd { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataUpdatedAtStart { get; set; }
    [SearchType(SearchType.TimeRange)] public DateTime? DataUpdatedAtEnd { get; set; }
}