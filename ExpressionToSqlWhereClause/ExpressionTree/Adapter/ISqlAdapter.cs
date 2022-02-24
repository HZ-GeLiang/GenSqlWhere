using System.Reflection;

namespace ExpressionToSqlWhereClause.ExpressionTree.Adapter
{
    public interface ISqlAdapter
    {
        string FormatColumnName(MemberInfo memberInfo);
    }
}

