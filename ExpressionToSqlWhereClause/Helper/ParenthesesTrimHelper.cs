using ExpressionToSqlWhereClause.ExpressionTree;
using ExpressionToSqlWhereClause.ExtensionMethod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;

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
        /// 特殊占位符
        /// </summary>
        private const string sc = "✖";//Special characters

        /// <summary>
        /// 为 () 包裹的
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool IsParenthesesWarp(string str)
        {
            if (str == null || str.Length < 2)
            {
                return false;
            }
            if (str[0] == '(' && str[str.Length - 1] == ')')
            {
                //符合这个的不一定是 (...)  有可能是 (...) Or (...)
                if (GetPositions(str, out var _)[0].Right == str.Length - 1)
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <inheritdoc cref="IsParenthesesWarp(string)"/>
        private static bool IsParenthesesWarp(Position position, string str)
        {
            if (position.HasValue())
            {
                if (position.Left == 0 && position.Right == str.Length - 1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc cref="IsParenthesesWarp(string)"/>
        private static bool IsParenthesesWarp(StringBuilder sb)
        {
            if (sb == null || sb.Length < 2)
            {
                return false;
            }
            if (sb[0] == '(' && sb[sb.Length - 1] == ')')
            {
                return IsParenthesesWarp(sb.ToString());
            }
            return false;
        }

        /// <summary>
        /// 由多个条件组成的 条件  如 ... and/or ... 的这种
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static bool ContainsAndWithOr(string str)
        {
            if (str.Contains(SqlKeys.and))
            {
                return true;
            }
            if (str.Contains(SqlKeys.or))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 去掉()包裹的
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string TrimParenthesesWarp(string str)
        {
            if (str != null && str.Length > 2)
            {
                str = str.Substring(1, str.Length - 2);
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns>当 str 只包含 and 且没有 in / 函数 时  返回true</returns>
        public static bool CanTrimAll(string str)
        {
            if (str.Contains(SqlKeys.or)) //Or语句
            {
                return false;
            }
            if (str.Contains(SqlKeys.@in)) //In语句
            {
                return false;
            }
            if (new Regex("[a-zA-Z]\\(").IsMatch(str)) //函数语句
            {
                return false;
            }
            if (str.Contains("(") == false && str.Contains(")") == false) //没有 () 的str
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 解析sql , 然后去掉可以去除的()
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ParseTrimAll(string str)
        {
            var result = ParseTrimAllCore(str);

            if (CanTrimAll(result))
            {
                result = TrimAll(str);
                return result;
            }

            var positions = GetPositions(result, out var _);
            if (positions.Count == 0)
            {
                return result;
            }
            if (IsParenthesesWarp(positions.First(), result)) // () 包裹的
            {
                result = TrimParenthesesWarp(result);
                return result;
            }
            else
            {
                #region 判断 顶层的个个条件是不是都是 and 组成, 如果是, 处理一下, 参见 case_复杂版的表达式解析3
                //实现有问题, 不处理了.
                //List<Position> positionslv1 = new List<Position>() {
                //    positions[0]
                //};

                //for (int i = 1; i < positions.Count; i++)
                //{
                //    if (positions[i].Left > positionslv1.Last().Right)
                //    {
                //        positionslv1.Add(positions[i]);
                //    }
                //}

                //if (positionslv1.Count <= 1)
                //{
                //    return result;
                //}

                //var relationCondition = result;
                //for (int i = positionslv1.Count - 1; i >= 0; i--)
                //{
                //    relationCondition = relationCondition.Remove(positionslv1[i].Left, positionslv1[i].Right - positionslv1[i].Left + 1);
                //}
                //if (relationCondition.Contains(SqlKeys.or) == false)//positionslv1 之间都是 and 关联的
                //{
                //    StringBuilder sbResult = new StringBuilder();
                //    for (int i = 0; i < positionslv1.Count; i++)
                //    {
                //        if (IsParenthesesWarp(positionslv1[i].GetContent))
                //        {
                //            sbResult.Append(TrimParenthesesWarp(positionslv1[i].GetContent));
                //        }
                //        else
                //        {
                //            sbResult.Append(positionslv1[i].GetContent);
                //        }
                //        if (i != positionslv1.Count - 1)
                //        {
                //            sbResult.Append(SqlKeys.and);
                //        }
                //    }

                //    result = sbResult.ToString();
                //    return result;
                //}
                #endregion

                return result; //不在单元测试中, 应该是太复杂了, 不知道怎么处理  
            }

        }

        private static void ParseWhere(List<Position> positions, string where, out List<string> parts, out List<string> partLinks)
        {
            parts = new List<string>();
            partLinks = new List<string>();

            var index = 0;
            for (int i = 0; i < positions.Count; i++)
            {
                #region 为了程序易读,这里的优化取消了
                //if (i == 0)
                //{
                //    if (where[position[0].Left] == '(' &&
                //        where[position[0].Right] == ')')
                //    {
                //        continue;
                //    }
                //} 
                #endregion

                if (index >= positions[i].Right)
                {
                    continue;
                }

                int partLeft = positions[i].Left;
                int partCount = positions[i].Right - positions[i].Left + 1;
                string part = where.Substring(partLeft, partCount);

                if (i == 0 && IsParenthesesWarp(part))//这个if为上面的注释部分
                {
                    continue;
                }

                parts.Add(part);

#if DEBUG
                var leftContent = positions[i].GetContent;//方便调试的时候查看

#endif
                int partLinkLeft = positions[i].Right + 1;

                var nextPosition = positions.FirstOrDefault(a => a.Left > partLinkLeft);

                /*
                 * while  的说明
                针对 (... And ((... And ...) Or (... And ...))) And (... Or ...) 这种情况做处理
                上面的示例:    partLinkLeft 值是在|处           |也就是这里
                然后 nextPosition.Left是在这里                       |在这里
                此时 结果为  ) And ,但是  ) And的 ) 是属于 position[0]的. 所以用来+1来跳过
                */
                while (partLinkLeft < nextPosition.Left && positions.Any(a => a.Right == partLinkLeft))
                {
                    partLinkLeft += 1;
                }

                if (nextPosition.HasValue())//最后一个是没有的.所以有可能是 default
                {
#if DEBUG
                    var rightContent = nextPosition.GetContent; //方便调试的时候查看
#endif

                    int partLinkCount = nextPosition.Left - partLinkLeft;
                    string partLink = where.Substring(partLinkLeft, partLinkCount);
                    partLinks.Add(partLink);
                }

                index = positions[i].Right;
            }
        }

        private static string ParseTrimAllCore(string where)
        {
            var positions = GetPositions(where, out var _);

            StringBuilder sb = new StringBuilder(where);

            foreach (var position in positions)
            {
                sb.Replace("(", sc, position.Left, 1);
                sb.Replace(")", sc, position.Right, 1);
            }

            var result = sb.ToString();

            if (result.Contains(SqlKeys.or) == false)
            {
                result = result.Replace(sc, "");
                return result;
            }
            else
            {
                sb.Clear();
                ParseWhere(positions, where, out var parts, out var partLinks);//  //获得各个部分的
                var partLinksContainsOr = partLinks.Contains(SqlKeys.or);
                for (int i = 0; i < parts.Count; i++)
                {
                    if (i > 0)
                    {
                        sb.Append(partLinks[i - 1]);
                    }
                    var trimResult = ParseTrimAllCore(parts[i]);

                    if (partLinksContainsOr || trimResult.Contains(SqlKeys.or))
                    {
                        if (partLinksContainsOr == true && (ContainsAndWithOr(trimResult) == false))
                        {
                            //((Name = @Name) Or (Name Like @Name1))  中的  (Name = @Name)  这个部分,
                            //在返回的时候不需要用()包裹 ,但是要在最后返回的时候添加()
                            sb.Append(trimResult);
                        }
                        else
                        {
                            if (IsParenthesesWarp(trimResult))
                            {
                                sb.Append(trimResult);
                            }
                            else
                            {
                                sb.Append("(").Append(trimResult).Append(")");
                            }
                        }
                    }
                    else
                    {
                        sb.Append(trimResult);
                    }
                }

                if (sb.Length > 0)
                {
                    // 在最后返回的时候添加()
                    if (partLinksContainsOr && (IsParenthesesWarp(sb) == false))
                    {
                        result = sb.Insert(0, "(").Append(")").ToString();
                    }
                    else
                    {
                        result = sb.ToString();
                    }

                    if (result.Contains(sc))
                    {
                        result = result.Replace(sc, "");
                    }

                    return result;
                }

                return where;//兜底,效果: 什么都没干
            }
        }

        /// <summary>
        /// 获得 () 的位置信息
        /// </summary>
        /// <param name="str"></param>
        /// <param name="unmatched"></param>
        /// <returns></returns>
        private static List<Position> GetPositions(string str, out List<int> unmatched)
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
#if DEBUG
                        list.Add(new Position(index, i, str));
#else
                        list.Add(new Position(index, i));
#endif
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

        /// <summary>
        /// ()的当前行的列位置
        /// </summary>
        internal struct Position
        {
            public int Left { get; set; }
            public int Right { get; set; }

#if DEBUG
            //DEBUG是为了方便调试用的

            public Position(int left, int right, string str)
            {
                this.Left = left;
                this.Right = right;
                this.Str = str;
            }

            public string GetContent => this.Str.Substring(this.Left, this.Right - this.Left + 1);


            public string Str { get; set; }

#else
            public Position(int left, int right)
            {
                this.Left = left;
                this.Right = right;
            }
#endif
            public bool HasValue()
            {
                return this.Left != this.Right;
            }
        }
    }
}
