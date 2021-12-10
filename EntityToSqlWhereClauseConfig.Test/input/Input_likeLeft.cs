namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_likeLeft
    {
        public string Url { get; set; }
        public string Data_Remark { get; set; }
    }

    public class Input_likeLeft_Attr
    {
        [SearchType(SearchType.likeLeft)] public string Url { get; set; }
        [SearchType(SearchType.likeLeft)] public string Data_Remark { get; set; }
    }
}
