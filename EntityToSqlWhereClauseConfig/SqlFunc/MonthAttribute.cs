using System;
using System.Collections.Generic;
using System.Text;

namespace EntityToSqlWhereClauseConfig.SqlFunc
{
    //标记接口
    public class SqlFuncAttribute : Attribute { }

    public class MonthAttribute : SqlFuncAttribute
    {
    }

    /// <summary>
    /// 最后的sql 为:  month in (xxx)
    /// </summary>
    public class MonthInAttribute : SqlFuncAttribute
    {
    }
}
