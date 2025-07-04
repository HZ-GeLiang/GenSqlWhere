﻿using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_lt_Attr
{
    [SearchType(SearchType.Lt)] public long? Id { get; set; }
    [SearchType(SearchType.Lt)] public DateTime? DataCreatedAt { get; set; }
}