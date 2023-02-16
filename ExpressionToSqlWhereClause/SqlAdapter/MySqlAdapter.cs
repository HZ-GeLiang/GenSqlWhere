using System.Reflection;
using ExpressionToSqlWhereClause.ExpressionTree.Adapter;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause
{
    public class MySqlAdapter : ISqlAdapter
    {
        /// <inheritdoc/>
        public virtual string FormatColumnName(string name)
        {
            return $"`{name}`";
        }
    }
}
