﻿using ExpressionToSqlWhereClause.EntitySearchBuilder;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_like_Attr
{
    [SearchType(SearchType.Like)] public string Url { get; set; }
    [SearchType(SearchType.Like)] public string Data_Remark { get; set; }
}