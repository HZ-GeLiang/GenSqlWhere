namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_neq
    {
        public bool IsDel { get; set; }
    }
    
    public class Input_neq_Attr
    {
        [SearchType(SearchType.neq)] public bool IsDel { get; set; }
    }
}
