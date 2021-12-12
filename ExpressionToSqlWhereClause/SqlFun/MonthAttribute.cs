using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionToSqlWhereClause.SqlFun
{

    public class DbFunctions
    {

        [DbFunctionAttribute("SqlServer", "Month")]
        public static Int32? Month(string expression)
        {
            throw new InvalidOperationException(nameof(Month));
            //throw new NotSupportedException(nameof(Month));
        }
    }

}
