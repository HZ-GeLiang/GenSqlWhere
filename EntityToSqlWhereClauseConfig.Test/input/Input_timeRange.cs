namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_timeRange
    {
        public System.DateTime? CreateAtStart { get; set; }
        public System.DateTime? CreateAtEnd { get; set; }
    }

    public class Input_timeRangeAttr
    {
        [SearchType(SearchType.datetimeRange)] public System.DateTime? CreateAtStart { get; set; }
        [SearchType(SearchType.datetimeRange)] public System.DateTime? CreateAtEnd { get; set; }

    }
}
