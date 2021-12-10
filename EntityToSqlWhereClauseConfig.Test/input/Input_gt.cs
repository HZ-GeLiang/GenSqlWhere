using System;

namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_gt
    {
        public long? Id { get; set; }
        public DateTime? DataCreatedAt { get; set; }
    }
    public class Input_gt_Attr
    {
        [SearchType(SearchType.gt)]
        public long? Id { get; set; }
        [SearchType(SearchType.gt)]
        public DateTime? DataCreatedAt { get; set; }
    }
}
