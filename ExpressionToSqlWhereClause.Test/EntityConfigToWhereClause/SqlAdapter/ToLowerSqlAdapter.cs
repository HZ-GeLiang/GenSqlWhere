using ExpressionToSqlWhereClause.ExpressionTree.Adapter;

namespace ExpressionToSqlWhereClaus.Test;

public class ToLowerSqlAdapter : DefaultSqlAdapter
{
    /// <inheritdoc/>
    public override string FormatColumnName(string name)
    {
        return name.ToLower();
    }
}