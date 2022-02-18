using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSqlWhereClause.SqlFunc
{
    // 示例 DbFunctionAttribute的 format 不要写() ,因为whereClause全是 and时,  () 要优化,
    // 此时有问题. 所以先约定这里写[] , 由程序在最后把 Month[xx]变成 Month()
    public class DbFunctions
    {
        //[DbFunctionAttribute("Month({0})")]
        [DbFunctionAttribute("Month[{0}]")]
        public static int Month(DateTime dt) { throw new InvalidOperationException(nameof(Month)); } // Expression 用 

 
        [DbFunctionAttribute("Month[{0}]")]
        public static List<int> MonthIn(DateTime dt) { throw new InvalidOperationException(nameof(MonthIn)); }


    }
}