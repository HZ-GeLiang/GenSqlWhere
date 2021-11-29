using System.Reflection;

namespace ExpressionToSqlWhereClause
{
    public class DefaultSqlAdapter : ISqlAdapter
    {
        public string FormatColumnName(MemberInfo memberInfo)
        {
            return memberInfo.Name;
        }
    }
}
