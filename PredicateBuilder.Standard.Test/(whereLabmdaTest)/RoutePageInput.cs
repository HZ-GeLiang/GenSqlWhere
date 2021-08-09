using System;

namespace PredicateBuilder.Standard.Test
{
    public class RoutePageInput 
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
}
