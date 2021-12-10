using System;

namespace EntityToSqlWhereClauseConfig.Test.input
{
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
    public class Input_Demo_Attr
    {
        [SearchType(SearchType.@in)] public string Id { get; set; }
        [SearchType(SearchType.like)] public string Url { get; set; }
        [SearchType(SearchType.@in)] public string Sex { get; set; }
        [SearchType(SearchType.eq)] public bool IsDel { get; set; }
        [SearchType(SearchType.like)] public string Data_Remark { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataCreatedAtStart { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataCreatedAtEnd { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataUpdatedAtStart { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataUpdatedAtEnd { get; set; }
    }


}
