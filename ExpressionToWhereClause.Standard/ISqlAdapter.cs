using System.Reflection;

namespace ExpressionToWhereClause.Standard
{
    public interface ISqlAdapter
    {
        string FormatColumnName(MemberInfo memberInfo);
    }
}

