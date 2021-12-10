namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_eq
    {
        public bool IsDel { get; set; }
    }

    public class Input_eq_Attr
    {
        [SearchType(SearchType.eq)]
        public bool IsDel { get; set; }
    }

}
