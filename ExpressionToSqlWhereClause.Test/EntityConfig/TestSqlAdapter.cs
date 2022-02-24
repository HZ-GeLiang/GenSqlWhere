using System.Reflection;
using ExpressionToSqlWhereClause.ExpressionTree.Adapter;

namespace SqlWhere.EntityConfig
{

    public class TestSqlAdapter : DefaultSqlAdapter
    {
        public override string FormatColumnName(MemberInfo memberInfo)
        {
            //return memberInfo.Name.ToLower();
            return base.FormatColumnName(memberInfo);
        }
    }

    public class ToLowerSqlAdapter : DefaultSqlAdapter
    {
        public override string FormatColumnName(MemberInfo memberInfo)
        {
            return memberInfo.Name.ToLower();
        }
    }
}
