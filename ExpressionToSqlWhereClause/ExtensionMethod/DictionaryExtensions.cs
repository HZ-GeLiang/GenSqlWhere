using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.ExtensionMethod
{
    internal static class DictionaryExtensions
    {
        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            Dictionary<TKey, TValue> clone = new Dictionary<TKey, TValue>();
            foreach (var key in dict.Keys)
            {
                clone.Add(key, dict[key]);
            }
            return clone;
        }
    }
}
