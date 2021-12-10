namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_likeRight
    {
        public string Url { get; set; }
        public string Data_Remark { get; set; }
    }
 
    public class Input_likeRight_Attr
    {
        [SearchType(SearchType.likeRight)] public string Url { get; set; }
        [SearchType(SearchType.likeRight)] public string Data_Remark { get; set; }
    }
}
