﻿using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.SqlFunc.EntityConfig;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_sqlFun_Month_eq
{
    [Month]
    [SearchType(SearchType.Eq)]
    public int DataCreatedAt { get; set; } // 必须和 Entity 的属性名一致
}