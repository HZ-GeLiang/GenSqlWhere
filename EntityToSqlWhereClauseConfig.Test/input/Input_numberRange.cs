namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_numberRange
    {
        public int? Id { get; set; } 
    }

    public class Input_numberRange_Attr
    {
        [SearchType(SearchType.numberRange)] public int? IdLeft { get; set; }
        [SearchType(SearchType.numberRange)] public int? IdRight { get; set; }
    }



    public class Input_numberRange2
    {
        public int? Id { get; set; }
    }
    public class Input_numberRange2_Attr
    {
        [SearchType(SearchType.numberRange)] public int? Id { get; set; }

    }
}
