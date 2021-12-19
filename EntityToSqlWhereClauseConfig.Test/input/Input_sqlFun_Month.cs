using System;
using System.Collections.Generic;
using EntityToSqlWhereClauseConfig.SqlFunc;

namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_sqlFun_Month
    {
        [Month]
        public int DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }

    public class Input_sqlFun_Month2
    {
        [MonthIn]
        public List<int> DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }

    public class Input_sqlFun_Month3
    {
        [MonthIn]
        public string DataCreatedAt { get; set; } //最后要翻译为 List<int> 
    }
    public class Input_sqlFun_Month4
    {
        [MonthIn]
        public int DataCreatedAt { get; set; } //最后要翻译为 List<int> 
    }
    public class Input_sqlFun_Month5
    {
        [MonthIn]
        public List<string> DataCreatedAt { get; set; } //最后要翻译为 List<int> 
    }
}
