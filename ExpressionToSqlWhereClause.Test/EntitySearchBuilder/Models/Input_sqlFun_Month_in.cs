using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.SqlFunc.EntityConfig;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Input_sqlFun_Month_in
{
    [Month]
    [SearchType(SearchType.In)]
    public int DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
}