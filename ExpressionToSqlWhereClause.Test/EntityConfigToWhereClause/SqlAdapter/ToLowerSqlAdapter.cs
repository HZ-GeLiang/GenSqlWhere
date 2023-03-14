using ExpressionToSqlWhereClause.ExpressionTree.Adapter;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause.SqlAdapter
{

    public class ToLowerSqlAdapter : DefaultSqlAdapter
    {
        /// <inheritdoc/>
        public override string FormatColumnName(string name)
        {
            return name.ToLower();
        }
    }
}
