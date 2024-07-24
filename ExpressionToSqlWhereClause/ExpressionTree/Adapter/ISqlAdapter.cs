namespace ExpressionToSqlWhereClause.ExpressionTree.Adapter
{
    public interface ISqlAdapter
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="name">属性名</param>
        /// <returns></returns>
        string FormatColumnName(string name);
    }
}