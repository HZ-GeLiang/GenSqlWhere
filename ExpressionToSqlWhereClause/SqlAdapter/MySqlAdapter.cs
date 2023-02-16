using System.Reflection;
using ExpressionToSqlWhereClause.ExpressionTree.Adapter;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause
{
    /// <summary>
    /// sqlserver
    /// </summary>
    public class MySqlAdapter : ISqlAdapter
    {
        /// <inheritdoc/>
        public virtual string FormatColumnName(string name)
        {
            return $"`{name}`";
        }
    }
}
