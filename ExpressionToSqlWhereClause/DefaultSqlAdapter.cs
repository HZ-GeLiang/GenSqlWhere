using System.Reflection;

namespace ExpressionToSqlWhereClause
{
    public class DefaultSqlAdapter : ISqlAdapter
    {
        public virtual string FormatColumnName(MemberInfo memberInfo)
        {
            return memberInfo.Name;
        }
    }
}
