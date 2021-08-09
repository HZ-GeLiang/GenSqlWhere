using System.Reflection;

namespace ExpressionToWhereClause.Standard
{
    public class DefaultSqlAdapter : ISqlAdapter
    {
        public string FormatColumnName(MemberInfo memberInfo)
        {
            return memberInfo.Name;
        }
    }
}
