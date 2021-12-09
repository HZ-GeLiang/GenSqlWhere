using System.Reflection;

namespace ExpressionToSqlWhereClause
{
    public interface ISqlAdapter
    {
         string FormatColumnName(MemberInfo memberInfo);
    }
}

