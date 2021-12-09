using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionToSqlWhereClause.Test
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
