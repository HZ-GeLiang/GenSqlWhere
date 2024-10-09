using ExpressionToSqlWhereClause.ExpressionTree.Adapter;

namespace ExpressionToSqlWhereClause.Test;

public class TestSqlAdapter : DefaultSqlAdapter
{
    public override string FormatColumnName(string name)
    {
        //return name.ToLower();
        return base.FormatColumnName(name);
    }
}