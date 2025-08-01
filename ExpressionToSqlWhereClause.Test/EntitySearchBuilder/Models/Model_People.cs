﻿namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;

public class Model_People
{
    public int Id { get; set; }

    public string Url { get; set; }

    public sbyte Sex { get; set; }
    public bool IsDel { get; set; }
    public string DataRemark { get; set; }
    public DateTime DataCreatedAt { get; set; }
    public DateTime DataUpdatedAt { get; set; }
}