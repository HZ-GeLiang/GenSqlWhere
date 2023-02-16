using ExpressionToSqlWhereClause.ExpressionTree;
using ExpressionToSqlWhereClause.ExtensionMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ExpressionToSqlWhereClause.Helper
{
    /// <summary>
    /// 去除括弧帮助类
    /// </summary>
    internal sealed class ParenthesesTrimHelper
    {
        /// <summary>
        /// 去掉所有的()
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static string TrimAll(string where)
        {
            if (where.Contains("("))
            {
                where = where.Replace("(", string.Empty);
            }
            if (where.Contains(")"))
            {
                where = where.Replace(")", string.Empty);
            }
            return where;
        }


        /// <summary>
        /// 解析sql , 然后去掉可以去除的()
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public static string ParseTrimAll(string where)
        {
            var position = GetPosition(where, out var _);

            StringBuilder sb = new StringBuilder(where);

            foreach (var item in position)
            {
                sb.Replace("(", "√", item.Left, 1);
                sb.Replace(")", "√", item.Right, 1);
            }

            var result = sb.ToString();

            if (result.Contains(SqlKeys.or) == false)
            {
                result = result.Replace("√", "");
                return result;
            }
            

            return where;
        }


        private static List<Position> GetPosition(string str, out List<int> unmatched)
        {
            //string str = "((A + B) * (C - D) + E) / F";

            List<Position> list = new List<Position>();
            Stack<int> stack = new Stack<int>();
            //List<int> unmatched = new List<int>();
            unmatched = new List<int>();

            var inSql = $" {SqlKeys.@in} ";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '(')
                {
                    stack.Push(i);
                }
                else if (str[i] == ')')
                {
                    if (stack.Count > 0)
                    {
                        int index = stack.Pop();

                        // ( 的前面4个字符为 ' in ' , 说明是In语句
                        if (index - 4 > 0 &&
                            inSql.IsEqualIgnoreCase(str.Substring(index - 4, 4))
                            )
                        {
                            continue;
                        }

                        // ( 的前面1个字符为字母, 说明是函数语句
                        if (index - 1 > 0 &&
                            Char.IsLetter(str.Substring(index - 1, 1)[0])
                           )
                        {
                            continue;
                        }

                        //Console.WriteLine($"Left: {index}, Right: {i}");
                        list.Add(new Position(index, i));
                    }
                    else
                    {
                        unmatched.Add(i);
                    }
                }
            }

            while (stack.Count > 0)
            {
                int index = stack.Pop();
                unmatched.Add(index);
            }
            //Console.WriteLine("Unmatched: " + string.Join(", ", unmatched));

            if (unmatched.Any()) //str 有误
            {
                return new List<Position>();
            }


            var result = list.OrderBy(a => a.Left).ToList();

            return result;
        }
        internal struct Position
        {
            public Position(int left, int right)
            {
                this.Left = left;
                this.Right = right;
            }
            public int Left { get; set; }
            public int Right { get; set; }
        }
    }


}
