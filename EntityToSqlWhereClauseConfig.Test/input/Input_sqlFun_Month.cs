using System;
using EntityToSqlWhereClauseConfig.SqlFunc;

namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_sqlFun_Month
    {
        [Month]
        public int DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
    }


}
