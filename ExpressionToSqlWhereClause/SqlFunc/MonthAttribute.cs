using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSqlWhereClause.SqlFunc
{
    public class DbFunctions
    {
        //[DbFunctionAttribute("Month({0})")]
        [DbFunctionAttribute("Month[{0}]")] // 不要写() ,因为where clause 全是 () and () 的时候要优化() ,程序最后会把 Month[xx]变成 Month()
        public static Int32 Month(DateTime dt)
        {
            throw new InvalidOperationException(nameof(Month));
        }
    }
}