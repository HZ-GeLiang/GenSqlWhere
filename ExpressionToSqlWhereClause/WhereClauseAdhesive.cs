using System.Collections.Generic;

namespace ExpressionToSqlWhereClause
{
    /// <summary>
    /// Where子句胶粘剂
    /// </summary>
    public class WhereClauseAdhesive
    {
        public WhereClauseAdhesive(ISqlAdapter sqlAdapter, Dictionary<string, object> parameters)
        {
            Parameters = parameters;
            SqlAdapter = sqlAdapter;
        }

        public Dictionary<string, object> Parameters { get; }

        public ISqlAdapter SqlAdapter { get; }
    }
}
