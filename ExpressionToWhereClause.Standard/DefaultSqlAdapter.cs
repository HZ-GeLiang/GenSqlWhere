using System.Reflection;

namespace ExpressionToWhereClause
{
    public class DefaultSqlAdapter : ISqlAdapter
    {
        public string FormatColumnName(MemberInfo memberInfo)
        {
            return memberInfo.Name;
        }
    }
}
