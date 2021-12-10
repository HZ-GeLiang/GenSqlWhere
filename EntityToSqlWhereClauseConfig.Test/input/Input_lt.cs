using System;

namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_lt
    {
        public long? Id { get; set; }
        public DateTime? DataCreatedAt { get; set; }
    }

    public class Input_lt_Attr
    {
        [SearchType(SearchType.lt)] public long? Id { get; set; }
        [SearchType(SearchType.lt)] public DateTime? DataCreatedAt { get; set; }
    }
}
