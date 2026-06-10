using ExpressionToSqlWhereClause.ExpressionTree.Adapter;

namespace ExpressionToSqlWhereClauseTest;

public class TestSqlAdapter : DefaultSqlAdapter
{
    public override string FormatColumnName(string name)
    {
        //return name.ToLower();
        return base.FormatColumnName(name);
    }
}