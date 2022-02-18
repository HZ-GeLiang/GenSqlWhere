using System;

namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_ge
    {
        public long? Id { get; set; }
        public DateTime? DataCreatedAt { get; set; }
    }

    public class Input_ge_Attr
    {
        [SearchType(SearchType.ge)] public long? Id { get; set; }
        [SearchType(SearchType.ge)] public DateTime? DataCreatedAt { get; set; }
    }
}
