using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.ExtensionMethods
{
    public static class StringExtensions
    {
        public static bool HasValue(this string value) => !string.IsNullOrWhiteSpace(value);

        public static List<int> SplitToInt(this string value, char separator)
        {
            if (value.HasValue() == false)
            {
                return new List<int>();
            }
            return value.SplitToInt(separator, true);
        }

        /// <summary>
        /// 分隔后转int
        /// </summary>
        /// <param name="value">内容</param>
        /// <param name="separator">分隔符</param>
        /// <param name="distinct">是否去重复</param>
        /// <returns></returns>
        public static List<int> SplitToInt(this string value, char separator, bool distinct)
        {
            if (value.HasValue() == false)
            {
                return new List<int>();
            }
            var splitResult = value.SplitToArray(distinct, StringSplitOptions.RemoveEmptyEntries, new[] { separator });

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

        public static string[] SplitToArray(this string value, bool needDistinct, StringSplitOptions stringSplitOption, params char[] separator)
        {
            if (value.HasValue() == false)
            {
                return new string[0];
            }
            var result = value.Split(separator, stringSplitOption);
            if (needDistinct)
            {
                return result.Distinct().ToArray();
            }
            else
            {
                return result;
            }
        }
    }
}