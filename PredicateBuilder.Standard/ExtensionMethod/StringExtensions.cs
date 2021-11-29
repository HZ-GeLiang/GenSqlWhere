namespace PredicateBuilder.ExtensionMethod
{
    internal static class StringExtensions
    {
        /// <summary>
        /// 移除String对象的最后面几个指定字符
        /// </summary>
        /// <param name="value"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string RemoveSuffix(this string value, string suffix)
        {
            if (value == null) return null;
            if (suffix == null || suffix.Length <= 0) return value;
            return value.EndsWith(suffix) ? value.Substring(0, value.Length - suffix.Length) : value;
        }
    }
}