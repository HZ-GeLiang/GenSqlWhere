using System.Reflection;

namespace ExpressionToSqlWhereClause.ExpressionTree.Adapter
{
    public class DefaultSqlAdapter : ISqlAdapter
    {

        /// <inheritdoc/>
        public virtual string FormatColumnName(string name)
        {
            // System.Diagnostics.Debugger.Break();
            return name;
        }
    }
}
