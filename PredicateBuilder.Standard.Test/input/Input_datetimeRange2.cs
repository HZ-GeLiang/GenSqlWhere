using System;

namespace ExpressionToSqlWhereClause.Test.input
{
    public class Input_datetimeRange 
    {
        public DateTime? DataCreatedAtStart { get; set; }
        public DateTime? DataCreatedAtEnd { get; set; }
        public DateTime? DataUpdatedAtStart { get; set; }
        public DateTime? DataUpdatedAtEnd { get; set; }
    }
    public class Input_datetimeRange2
    {
        public DateTime? DataCreatedAt  { get; set; }  
    }
}
