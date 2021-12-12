using System;
using System.Collections;
using System.Text;

namespace ExpressionToSqlWhereClause
{
    internal static class StringBuilderExtensions
    {
        /// <summary>
        /// 返回-1表示找不到
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int IndexOf(this StringBuilder sb, char value)
        {
            if (sb == null)
            {
                return -1;
            }
            for (int i = 0; i < sb.Length; i++)
            {
                if (sb[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }

    }
}