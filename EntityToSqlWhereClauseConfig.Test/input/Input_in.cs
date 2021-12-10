namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_in
    {
        //public int? Id { get; set; }
        public string Id { get; set; }
        public string Sex { get; set; }
      
    }
    public class Input_in2
    {
       public int? Id { get; set; }
       
    }

    public class Input_in_Attr
    {
        [SearchType(SearchType.@in)] public string Id { get; set; }
        [SearchType(SearchType.@in)] public string Sex { get; set; }

    }
    public class Input_in2_Attr
    {
        [SearchType(SearchType.@in)] public int? Id { get; set; }

    }

}
