using System;

namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_datetimeRange
    {
        public DateTime? DataCreatedAt { get; set; }
        public DateTime? DataUpdatedAt { get; set; }
    }


    public class Input_datetimeRange_Attr
    {
        [SearchType(SearchType.datetimeRange)] public DateTime? DataCreatedAtStart { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataCreatedAtEnd { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataUpdatedAtStart { get; set; }
        [SearchType(SearchType.datetimeRange)] public DateTime? DataUpdatedAtEnd { get; set; }
    }

    public class Input_datetimeRange2_Attr
    {
        [SearchType(SearchType.datetimeRange)] public DateTime? DataCreatedAt { get; set; }
    }

}
