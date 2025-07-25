﻿using System.Data;
using System.Text;

namespace ExpressionToSqlWhereClause.ExtensionMethods
{
    internal static class AggregateExtensions
    {
        #region 常见的列表对象调用 AggregateToString, 表现形式一般为 list.AggregateToString(a => a,",")

        public static string AggregateToString<TKey, TValue>(this Dictionary<TKey, TValue>.KeyCollection keys, string separator)
        {
            var keyType = typeof(TKey);
            if (keyType.IsClass == false || keyType == typeof(string))
            {
                return keys.ToList().AggregateToString(null, a => a, separator);
            }
            throw new Exception($"不支持的TKey类型:{keyType}");
        }

        public static string AggregateToString<TKey, TValue>(this Dictionary<TKey, TValue>.KeyCollection keys, Func<TKey, bool> predicate, string separator)
        {
            var keyType = typeof(TKey);
            if (keyType.IsClass == false || keyType == typeof(string))
            {
                return keys.ToList().AggregateToString(predicate, a => a, separator);
            }
            throw new Exception($"不支持的TKey类型:{keyType}");
        }

        public static string AggregateToString(this List<string> source, string separator)
        {
            return source.Where(a => string.IsNullOrEmpty(a) == false).AggregateToString(a => a, separator);
        }

        public static string AggregateToString(this List<string> source, Func<string, bool> predicate, string separator)
        {
            return source.Where(a => string.IsNullOrEmpty(a) == false && predicate.Invoke(a)).AggregateToString(a => a, separator);
        }

        public static string AggregateToString(this IOrderedEnumerable<string> source, string separator)
        {
            return source.Where(a => string.IsNullOrEmpty(a) == false).AggregateToString(a => a, separator);
        }

        public static string AggregateToString(this IOrderedEnumerable<string> source, Func<string, bool> predicate, string separator)
        {
            return source.Where(a => string.IsNullOrEmpty(a) == false && predicate.Invoke(a)).AggregateToString(a => a, separator);
        }

        public static string AggregateToString(this List<int> source, string separator)
        {
            return source.AggregateToString(null, a => a, separator);
        }

        public static string AggregateToString(this List<int> source, Func<int, bool> predicate, string separator)
        {
            return source.AggregateToString(predicate, a => a, separator);
        }

        public static string AggregateToString(this List<int?> source, string separator)
        {
            return source.AggregateToString(a => a.HasValue, a => a, separator);
        }

        public static string AggregateToString(this List<int?> source, Func<int?, bool> predicate, string separator)
        {
            return source.AggregateToString(a => a.HasValue && predicate.Invoke(a), a => a, separator);
        }

        #endregion

        #region AggregateToString , 聚合为一个字符串

        public static string AggregateToString(this DataRowCollection source, string columnName, string separator)
        {
            return AggregateToString(source, null, columnName, separator);
        }

        /// <summary>
        /// 聚合为一个字符串
        /// </summary>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="columnName"></param>
        /// <param name="separator"></param>
        /// <returns>没有值时返回""</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string AggregateToString(this DataRowCollection source, Func<DataRow, bool> predicate, string columnName, string separator)
        {
            if (source is null)
            {
                return string.Empty;
                //throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
            }
            if (columnName.HasValue() == false)
            {
                throw new ArgumentNullException($"{nameof(columnName)}参数不能为空", nameof(columnName));
            }
            if (separator is null)
            {
                separator = "";
                //throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));
            }
            if (source.Count <= 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            if (separator.HasValue())
            {
                if (predicate == null)
                {
                    foreach (DataRow item in source)
                    {
                        sb.Append(item[columnName]).Append(separator);
                    }
                }
                else
                {
                    foreach (DataRow item in source)
                    {
                        if (predicate.Invoke(item))
                        {
                            sb.Append(item[columnName]).Append(separator);
                        }
                    }
                }
            }
            else
            {
                if (predicate == null)
                {
                    foreach (DataRow item in source)
                    {
                        sb.Append(item[columnName]);
                    }
                }
                else
                {
                    foreach (DataRow item in source)
                    {
                        if (predicate.Invoke(item))
                        {
                            sb.Append(item[columnName]);
                        }
                    }
                }
            }

            if (separator.HasValue())
            {
                sb.Replace(separator, string.Empty, sb.Length - separator.Length, separator.Length);
            }
            var txt = sb.ToString();
            return txt;
        }

        public static string AggregateToString(this DataRowCollection source, Func<DataRow, dynamic> content, string separator)
        {
            return AggregateToString(source, null, content, separator);
        }

        public static string AggregateToString(this DataRowCollection source, Func<DataRow, bool> predicate, Func<DataRow, dynamic> content, string separator)
        {
            if (source is null)
            {
                return string.Empty;
                //throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
            }
            if (content is null)
            {
                throw new ArgumentNullException($"{nameof(content)}参数不能为空", nameof(content));
            }
            if (separator is null)
            {
                separator = "";
                //throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));
            }

            if (source.Count <= 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            if (separator.HasValue())
            {
                if (predicate == null)
                {
                    foreach (DataRow row in source)
                    {
                        sb.Append(content.Invoke(row)).Append(separator);
                    }
                }
                else
                {
                    foreach (DataRow row in source)
                    {
                        if (predicate.Invoke(row) == true)
                        {
                            sb.Append(content.Invoke(row)).Append(separator);
                        }
                    }
                }
            }
            else
            {
                if (predicate == null)
                {
                    foreach (DataRow row in source)
                    {
                        sb.Append(content.Invoke(row));
                    }
                }
                else
                {
                    foreach (DataRow row in source)
                    {
                        if (predicate.Invoke(row) == true)
                        {
                            sb.Append(content.Invoke(row));
                        }
                    }
                }
            }

            if (separator.HasValue())
            {
                sb.Replace(separator, string.Empty, sb.Length - separator.Length, separator.Length);
            }
            var txt = sb.ToString();
            return txt;
        }

        /// <inheritdoc cref="AggregateToString{TSource}(IEnumerable{TSource}, Func{TSource, bool}, Func{TSource, TSource}, string, int, string)"/>
        public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource> content, string separator)
        {
            return AggregateToString(source, null, content, separator, 0, string.Empty);
        }

        /// <inheritdoc cref="AggregateToString{TSource}(IEnumerable{TSource}, Func{TSource, bool}, Func{TSource, TSource}, string, int, string)"/>
        public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource> content, string separator, int groupCount, string groupSeparator)
        {
            return AggregateToString(source, null, content, separator, groupCount, groupSeparator);
        }

        /// <summary>
        /// 聚合为一个字符串
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="content"></param>
        /// <param name="separator"></param>
        /// <param name="groupCount">每个分组的数量</param>
        /// <param name="groupSeparator">分组的结尾符号</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, TSource> content, string separator, int groupCount, string groupSeparator)
        {
            if (source is null)
            {
                return string.Empty;
                //throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
            }
            if (content is null)
            {
                throw new ArgumentNullException($"{nameof(content)}参数不能为空", nameof(content));
            }
            if (separator is null)
            {
                separator = "";
                //throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));
            }

            if (source.Any() == false)
            {
                return string.Empty;
            }

#if DEBUG
            if (source.GetType() == typeof(string)) // string is IEnumerable<TSource> == true
            {
                // 一般来说这个场景不存在,防止代码无意中写错, 这个方法不支持 string 调用
                throw new Exception("字符串类型的变量不允许调用该方法");
            }
#endif

            var sb = new StringBuilder();
            var separatorhasValue = !string.IsNullOrEmpty(separator);

            if (groupCount > 0 && groupSeparator.HasValue())
            {
                var count = 0;
                var appendGroupSeparator = false;

                if (separatorhasValue)
                {
                    if (predicate == null)
                    {
                        foreach (var item in source)
                        {
                            if (appendGroupSeparator)
                            {
                                sb.Append(groupSeparator);
                                appendGroupSeparator = false;
                            }
                            count++;

                            sb.Append(content.Invoke(item)).Append(separator);
                            if (count > 0 && count % groupCount == 0)
                            {
                                appendGroupSeparator = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in source)
                        {
                            if (predicate.Invoke(item))
                            {
                                if (appendGroupSeparator)
                                {
                                    sb.Append(groupSeparator);
                                    appendGroupSeparator = false;
                                }
                                count++;

                                sb.Append(content.Invoke(item)).Append(separator);
                                if (count > 0 && count % groupCount == 0)
                                {
                                    appendGroupSeparator = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (predicate == null)
                    {
                        foreach (var item in source)
                        {
                            if (appendGroupSeparator)
                            {
                                sb.Append(groupSeparator);
                                appendGroupSeparator = false;
                            }
                            count++;
                            sb.Append(content.Invoke(item));
                            if (count > 0 && count % groupCount == 0)
                            {
                                appendGroupSeparator = true;
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in source)
                        {
                            if (predicate.Invoke(item))
                            {
                                if (appendGroupSeparator)
                                {
                                    sb.Append(groupSeparator);
                                    appendGroupSeparator = false;
                                }
                                count++;
                                sb.Append(content.Invoke(item));
                                if (count > 0 && count % groupCount == 0)
                                {
                                    appendGroupSeparator = true;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                if (separatorhasValue)
                {
                    if (predicate == null)
                    {
                        foreach (var item in source)
                        {
                            sb.Append(content.Invoke(item)).Append(separator);
                        }
                    }
                    else
                    {
                        foreach (var item in source)
                        {
                            if (predicate.Invoke(item))
                            {
                                sb.Append(content.Invoke(item)).Append(separator);
                            }
                        }
                    }
                }
                else
                {
                    if (predicate == null)
                    {
                        foreach (var item in source)
                        {
                            sb.Append(content.Invoke(item));
                        }
                    }
                    else
                    {
                        foreach (var item in source)
                        {
                            if (predicate.Invoke(item))
                            {
                                sb.Append(content.Invoke(item));
                            }
                        }
                    }
                }
            }

            if (sb.Length <= 0)
            {
                return string.Empty;
            }

            if (separatorhasValue)
            {
                sb.Replace(separator, string.Empty, sb.Length - separator.Length, separator.Length);
            }

            var txt = sb.ToString();
            return txt;
        }

        public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, dynamic> content, string separator)
        {
            return AggregateToString(source, null, content, separator);
        }

        /// <summary>
        /// 聚合为一个字符串
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="content">dynamic的目的: 调用端不用刻意的使用 ToString()</param>
        /// <param name="separator"></param>
        /// <returns>没有值时返回""</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="Exception"></exception>
        public static string AggregateToString<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, dynamic> content, string separator)
        {
            if (source is null)
            {
                return string.Empty;
                //throw new ArgumentNullException($"{nameof(source)}参数不能为空", nameof(source));
            }
            if (content is null)
            {
                throw new ArgumentNullException($"{nameof(content)}参数不能为空", nameof(content));
            }
            if (separator is null)
            {
                separator = "";
                //throw new ArgumentNullException($"{nameof(separator)}参数不能为空", nameof(separator));
            }

            if (source.Any() == false)
            {
                return string.Empty;
            }

#if DEBUG
            if (source.GetType() == typeof(string)) // string is IEnumerable<TSource> == true
            {
                // 一般来说这个场景不存在,防止代码无意中写错, 这个方法不支持 string 调用
                throw new Exception("字符串类型的变量不允许调用该方法");
            }
#endif

            var sb = new StringBuilder();
            var separatorhasValue = !string.IsNullOrEmpty(separator);

            if (separatorhasValue)
            {
                if (predicate == null)
                {
                    foreach (var item in source)
                    {
                        sb.Append(content.Invoke(item)).Append(separator);
                    }
                }
                else
                {
                    foreach (var item in source)
                    {
                        if (predicate.Invoke(item))
                        {
                            sb.Append(content.Invoke(item)).Append(separator);
                        }
                    }
                }
            }
            else
            {
                if (predicate == null)
                {
                    foreach (var item in source)
                    {
                        sb.Append(content.Invoke(item));
                    }
                }
                else
                {
                    foreach (var item in source)
                    {
                        if (predicate.Invoke(item))
                        {
                            sb.Append(content.Invoke(item));
                        }
                    }
                }
            }

            if (sb.Length <= 0)
            {
                return string.Empty;
            }

            if (separatorhasValue)
            {
                sb.Replace(separator, string.Empty, sb.Length - separator.Length, separator.Length);
            }
            var txt = sb.ToString();
            return txt;
        }

        #endregion
    }
}