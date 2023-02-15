using System.Reflection;
using ExpressionToSqlWhereClause.ExpressionTree.Adapter;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause
{

    public class TestSqlAdapter : DefaultSqlAdapter
    {
        public override string FormatColumnName(string name)
        {
            //return name.ToLower();
            return base.FormatColumnName(name);
        }
    }

    public class ToLowerSqlAdapter : DefaultSqlAdapter
    {
        /// <inheritdoc/>
        public override string FormatColumnName(string name)
        {
            return name.ToLower();
        }
    }
}
