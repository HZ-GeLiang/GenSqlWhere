namespace ExpressionToSqlWhereClause.ExtensionMethods;

internal static class StringExtensions
{
    /// <summary>
    /// 是否有值
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool HasValue(this string value)
    {
        return !string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// 移除String对象的最后面几个指定字符
    /// </summary>
    /// <param name="value"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static string RemoveSuffix(this string value, string suffix)
    {
        if (value == null)
        {
            return null;
        }

        if (suffix == null || suffix.Length <= 0)
        {
            return value;
        }

        return value.EndsWith(suffix) ? value.Substring(0, value.Length - suffix.Length) : value;
    }

    /// <summary>
    /// 忽略大小写, (string)null 和 (string)null 比较 返回 true.
    /// </summary>
    /// <param name="strA"></param>
    /// <param name="strB"></param>
    /// <param name="comparisonType"></param>
    /// <returns></returns>
    public static bool IsEqualIgnoreCase(this string strA, string strB, StringComparison comparisonType = StringComparison.OrdinalIgnoreCase)
    {
        return string.Compare(strA, strB, comparisonType) == 0;
    }
}