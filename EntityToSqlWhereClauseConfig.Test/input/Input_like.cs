namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_like
    {
        public string Url { get; set; }
        public string Data_Remark { get; set; }
    }


    public class Input_like_Attr
    {
        [SearchType(SearchType.like)] public string Url { get; set; }
        [SearchType(SearchType.like)] public string Data_Remark { get; set; }
    }
}
