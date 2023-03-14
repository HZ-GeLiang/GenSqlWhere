using System;
using System.Collections;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.Helper
{
    internal sealed class TypeHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="types">提供参数个数 >1 时, 会异常</param>
        /// <returns></returns>
        public static IList MakeList(params Type[] types)
        {
            Type type = typeof(List<>).MakeGenericType(types);
            object list = Activator.CreateInstance(type);
            System.Collections.IList ilist = list as System.Collections.IList;
            return ilist;
        }

    }
}
