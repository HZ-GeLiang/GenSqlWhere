using System.Reflection;

namespace ExpressionToSqlWhereClause
{
    public class DefaultSqlAdapter : ISqlAdapter
    {
        public virtual string FormatColumnName(MemberInfo memberInfo)
        {
           // System.Diagnostics.Debugger.Break();
            return memberInfo.Name;
        }
    }
}
