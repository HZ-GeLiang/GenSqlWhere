using System.Reflection;

namespace ExpressionToWhereClause
{
    public interface ISqlAdapter
    {
        string FormatColumnName(MemberInfo memberInfo);
    }
}

