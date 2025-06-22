namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

//demo1, 使用   whereLambda[SearchType.like] 的这种配置方式来创建sql
//注: demo1 和demo2 如果写重复, 那么都会生效
public class Input_Demo
{
    //public int? Id { get; set; }
    public string Id { get; set; }

    public string Url { get; set; }
    public string Sex { get; set; }
    public bool IsDel { get; set; }
    public string Data_Remark { get; set; }
    public DateTime? DataCreatedAtStart { get; set; }
    public DateTime? DataCreatedAtEnd { get; set; }
    public DateTime? DataUpdatedAtStart { get; set; }
    public DateTime? DataUpdatedAtEnd { get; set; }
}