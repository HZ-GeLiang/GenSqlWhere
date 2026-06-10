using ExpressionToSqlWhereClause.ExpressionTree.Adapter;

namespace ExpressionToSqlWhereClauseTest;

public class ToLowerSqlAdapter : DefaultSqlAdapter
{
    /// <inheritdoc/>
    public override string FormatColumnName(string name)
    {
        return name.ToLower();
    }
}