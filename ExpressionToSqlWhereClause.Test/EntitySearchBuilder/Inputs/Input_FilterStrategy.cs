using ExpressionToSqlWhereClause.EntitySearchBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;

public class Input_FilterStrategy
{
    public int? Id { get; set; }
    public decimal? GetSum { get; set; }
    public string GetSumFilter { get; set; }
}