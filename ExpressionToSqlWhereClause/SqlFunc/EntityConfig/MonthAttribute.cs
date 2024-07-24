using System;

namespace ExpressionToSqlWhereClause.SqlFunc.EntityConfig
{
    //标记接口
    public class SqlFuncAttribute : Attribute
    { }

    //如果配合   ExpressionToSqlWhereClause , 那么仅支持 [SearchType(SearchType.eq | SearchType.neq)]
    public class MonthAttribute : SqlFuncAttribute
    { }

    /// <summary>
    /// 最后的sql 为:  month in (xxx)
    ///
    /// </summary>
    //如果配合   ExpressionToSqlWhereClause , 那么仅支持 [SearchType(SearchType.!in)]
    public class MonthInAttribute : SqlFuncAttribute
    { }
}