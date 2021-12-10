using EntityToSqlWhereClauseConfig.SqlFun;
using System;

namespace EntityToSqlWhereClauseConfig.Test.input
{
    public class Input_sqlFun_Month
    {
        [MonthAttribute]
        public DateTime DataCreatedAt { get; set; }
    }

   
}
