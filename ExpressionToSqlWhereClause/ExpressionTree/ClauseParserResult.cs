using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSqlWhereClause.ExpressionTree
{
    internal class ClauseParserResult
    {
        public string WhereClause { get; set; }
        public WhereClauseAdhesive Adhesive { get; set; }
    }
}
