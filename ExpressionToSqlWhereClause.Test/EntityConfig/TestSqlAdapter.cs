using ExpressionToSqlWhereClause.ExpressionTree.Adapter;
using System.Reflection;

namespace ExpressionToSqlWhereClause.Test.EntityConfig
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
