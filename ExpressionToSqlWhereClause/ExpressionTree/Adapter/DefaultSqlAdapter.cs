namespace ExpressionToSqlWhereClause.ExpressionTree.Adapter
{
    public class DefaultSqlAdapter : ISqlAdapter
    {
        /// <inheritdoc/>
        public virtual string FormatColumnName(string name)
        {
#if DEBUG
            //System.Diagnostics.Debugger.Break();
#endif
            return name;
        }
    }
}