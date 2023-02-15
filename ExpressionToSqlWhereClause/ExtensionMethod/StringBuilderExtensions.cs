using System;
using System.Collections;
using System.Text;

// ReSharper disable once CheckNamespace
namespace ExpressionToSqlWhereClause.ExtensionMethod
{
    internal static class StringBuilderExtensions
    {
        /// <summary>
        /// 返回-1表示找不到
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="value"></param>
        /// <param name="startIndex">从0开始</param>
        /// <returns></returns>
        public static int IndexOf(this StringBuilder sb, char value, int startIndex = 0)
        {
            if (sb == null)
            {
                return -1;
            }
            for (int i = startIndex; i < sb.Length; i++)
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