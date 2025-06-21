#pragma warning disable IDE0130

using System.Data;
using System.Text;
using System.Text.RegularExpressions;

namespace Infra.ExtensionMethods
{
    public static class SplitExtensions
    {
        #region SplitXXX

        /// <summary>
        /// 默认是去重的
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitNewLine(this string str)
        {
            var splits = str.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            return splits;
        }

        /// <summary>
        /// 默认是去重的
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static List<string> SplitToList(this string value, params char[] separator)
        {
            return value.HasValue()
                ? value.Split(separator, StringSplitOptions.RemoveEmptyEntries).Distinct(StringComparer.Ordinal).ToList()
                : new List<string>(0);
        }

        /// <summary>
        /// 默认是去重的
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string[] SplitToArray(this string value, params char[] separator)
        {
            return value.HasValue()
                ? value.Split(separator, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray()
                : new string[0];
        }

        public static List<string> SplitToListNoDistinct(this string value, params char[] separator)
        {
            return value.HasValue() == false
                ? value.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList()
                : new List<string>(0);
        }

        public static string[] SplitToArrayNoDistinct(this string value, params char[] separator)
        {
            return value.HasValue() == false
                ? (new string[0])
                : value.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToArray();
        }

        #endregion

        #region SplitToIntArray

        /// <inheritdoc cref="SplitToIntList(string, char, bool)"/>
        public static int[] SplitToIntArray(this string content, char separator_char)
        {
            //if (content.HasValue() == false)
            //{
            //    return Array.Empty<int>();
            //}
            //return SplitToIntList(content, separator_char, true).ToArray();

            //使用span
            var span = content.AsSpan();
            var separator = separator_char.ToString().AsSpan();
            var sepLen = separator.Length;
            var index = -1;
            var result = new List<int>();
            do
            {
                index = span.IndexOf(separator);
                if (index == -1)
                {
                    result.Add(int.Parse(span));
                }
                else
                {
                    var value = span.Slice(0, index);
                    result.Add(int.Parse(value));
                    span = span.Slice(index + sepLen);
                }
            }
            while (index != -1);
            return result.ToArray();
        }

        #endregion

        #region SplitToIntList

        /// <inheritdoc cref="SplitToIntList(string, char, bool)"/>
        public static List<int> SplitToIntList(this string content, char separator_char)
        {
            if (content.HasValue() == false)
            {
                return new List<int>();
            }
            return SplitToIntList(content, separator_char, true);
        }

        /// <summary>
        /// 分隔后转int
        /// </summary>
        /// <param name="value">内容</param>
        /// <param name="separator">分隔符</param>
        /// <param name="distinct">是否去重复</param>
        /// <returns></returns>
        public static List<int> SplitToIntList(this string value, char separator, bool distinct)
        {
            if (value.HasValue() == false)
            {
                return new List<int>();
            }
            var splitResult = distinct
                ? value.SplitToArray(separator)
                : value.SplitToArrayNoDistinct(separator);

            var list = new List<int>();
            foreach (var split in splitResult)
            {
                if (int.TryParse(split, out var val))
                {
                    list.Add(val);
                }
            }
            return list;
        }

        #endregion

        #region SplitWithXxx

        /// <summary>
        /// 以浮点数进行分隔
        /// </summary>
        /// <param name="value">从0开始的浮点数</param>
        /// <param name="reservedDelimiter">保留分隔符号</param>
        /// <returns></returns>
        public static IEnumerable<string> SplitWithFloatNumber(string value, bool reservedDelimiter)
        {
            var reg = reservedDelimiter ? @"([0-9]*\.[0-9]+|[0-9]+)" : @"[0-9]*\.[0-9]+|[0-9]+";
            var splites = Regex.Split(value, reg);

            for (int i = 0; i < splites.Length; i++)
            {
                if (i == 0 || i == splites.Length - 1)  // splites 的结果中, 第一项和最后一项 都有可能为空.
                {
                    if (!string.IsNullOrEmpty(splites[i]))
                    {
                        yield return splites[i];
                    }
                }
                else
                {
                    yield return splites[i];
                }
            }
        }

        /// <summary>
        /// 按数字(不含小数, 是自然数)和四则运算符
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reservedDelimiter"></param>
        /// <returns></returns>
        public static List<string> SplitWithNumberAndFourOperationalSymbols(this string value)
        {
            var list = new List<string>();
            foreach (var item in SplitWithNumber(value, true))
            {
                if (item.Length > 1)
                {
                    list.AddRange(SplitWithFourOperationalSymbols(item, true));
                }
                else
                {
                    list.Add(item);
                }
            }
            return list;
        }

        /// <summary>
        /// 按数字(不含小数, 是自然数)分隔
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reservedDelimiter">保留分隔符号</param>
        /// <returns></returns>
        public static IEnumerable<string> SplitWithNumber(this string value, bool reservedDelimiter)
        {
            var reg = reservedDelimiter ? @"(\d+)" : @"\d+";
            var splites = Regex.Split(value, reg);
            for (int i = 0; i < splites.Length; i++)
            {
                if (i == 0 || i == splites.Length - 1)  // splites 的结果中, 第一项和最后一项 都有可能为空.
                {
                    if (!string.IsNullOrEmpty(splites[i]))
                    {
                        yield return splites[i];
                    }
                }
                else
                {
                    yield return splites[i];
                }
            }
        }

        /// <summary>
        /// 四则运算符号的分隔
        /// </summary>
        /// <param name="value"></param>
        /// <param name="reservedDelimiter">保留分隔符号</param>
        /// <returns></returns>
        public static IEnumerable<string> SplitWithFourOperationalSymbols(this string value, bool reservedDelimiter)
        {
            var reg = reservedDelimiter ? @"([\+\-\*\\/]{1})" : @"[\+\-\*\\/]{1}";
            var splites = Regex.Split(value, reg);
            for (int i = 0; i < splites.Length; i++)
            {
                if (i == 0 || i == splites.Length - 1)  // splites 的结果中, 第一项和最后一项 都有可能为空.
                {
                    if (!string.IsNullOrEmpty(splites[i]))
                    {
                        yield return splites[i];
                    }
                }
                else
                {
                    yield return splites[i];
                }
            }
        }

        #endregion

        #region SplitMany

        public static List<TResult> SplitMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, string[]> selector)
        {
            var list = SplitManyCore<TSource, TResult>(source, selector);
            return list;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <param name="needDistinct">简单的去重</param>
        /// <returns></returns>
        public static List<TResult> SplitMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, string[]> selector, bool needDistinct)
        {
            var list = SplitManyCore<TSource, TResult>(source, selector);
            return needDistinct ? list.Distinct().ToList() : list;
        }

        private static List<TResult> SplitManyCore<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, string[]> selector)
        {
            var result = new List<TResult>();
            foreach (var item in source)
            {
                string[] SplitResult = selector.Invoke(item);// {1,2,3}
                foreach (var split in SplitResult)
                {
                    result.Add((TResult)Convert.ChangeType(split, typeof(TResult)));
                }
            }

            return result;
        }

        #endregion

        #region Split之后的xxx操作

        /// <summary>
        /// 移除 Split 之后的第一级数据.譬如 ."1,2,3" 那最后返回 "2,3"
        /// </summary>
        /// <param name="src"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string RemoveFirstLevelAfterSplit(this string src, string separator)
        {
            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            if (separator is null)
            {
                throw new ArgumentNullException(nameof(separator));
            }

            if (separator.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(separator));
            }

            int startIndex = src.IndexOf(separator, StringComparison.Ordinal);
            //return startIndex != -1 ? src.Substring(startIndex + 1, src.Length - 1 - startIndex) : src;
            if (startIndex == -1)
            {
                return src;
            }

            string result = src.Substring(startIndex + 1, src.Length - 1 - startIndex);
            return result;
        }

        /// <summary>
        /// 移除 Split 之后的最后一级数据.譬如 ."1,2,3" 那最后返回 "1,2"
        /// </summary>
        /// <param name="src"></param>
        /// <param name="separator"></param>
        public static string RemoveLastLevelAfterSplit(this string src, char separator)
        {
            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            if (separator <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(separator));
            }

            int endIndex = src.LastIndexOf(separator);
            return endIndex != -1 ? src.Substring(0, endIndex) : src;
        }

        public static string GetFirstLevelAfterSplit(this string src, string separator)
        {
            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            if (separator is null)
            {
                throw new ArgumentNullException(nameof(separator));
            }

            if (separator.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(separator));
            }

            int startIndex = src.IndexOf(separator, StringComparison.Ordinal);
            return startIndex != -1 ? src.Substring(0, startIndex) : src;
        }

        public static string GetLastLevelAfterSplit(this string src, char separator)
        {
            if (src is null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            if (separator <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(separator));
            }

            int lastIndex = src.LastIndexOf(separator);
            //return lastIndex != -1 ? src.Substring(lastIndex + 1, src.Length - lastIndex - 1) : src;
            if (lastIndex == -1)
            {
                return src;
            }

            var result = src.Substring(lastIndex + 1, src.Length - lastIndex - 1);
            return result;
        }

        /// <summary>
        /// 提取indexOf后面的字符
        /// </summary>
        /// <param name="src"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        public static string ExtractAfter(this string src, string value)
        {
            if (string.IsNullOrEmpty(src) || string.IsNullOrEmpty(value))
            {
                return src;
            }

            int index = src.IndexOf(value, StringComparison.OrdinalIgnoreCase);
            if (index != -1)
            {
                return src.Substring(index + value.Length);
            }

            return src;
        }

        #endregion
    }
}

#pragma warning restore IDE0130