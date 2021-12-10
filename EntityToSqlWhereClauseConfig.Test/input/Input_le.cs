using System;

namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_le
    {
        public long? Id { get; set; }
        public DateTime? DataCreatedAt { get; set; }
    }
  
    public class Input_le_Attr
    {
        [SearchType(SearchType.le)] public long? Id { get; set; }
        [SearchType(SearchType.le)] public DateTime? DataCreatedAt { get; set; }
    }
}
