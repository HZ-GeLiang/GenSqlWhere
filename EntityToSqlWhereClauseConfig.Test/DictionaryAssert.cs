using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    public class DictionaryAssert
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="dict">expectedParameters</param>
        /// <param name="dict2">actualParameters</param>
        public static void AreEqual<TKey, TValue>(Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> dict2)
        {
            if (dict2 == null)
            {
                Assert.AreEqual(dict, null);
                return;
            }
            if (dict == null)
            {
                Assert.Fail(nameof(dict) + "²»ÄÜÎª¿Õ");
                return;
            }

            Assert.AreEqual(dict.Count, dict2.Count);
            CollectionAssert.AreEqual(dict.Keys, dict2.Keys);
            foreach (var key in dict.Keys)
            {
                Assert.AreEqual(dict[key], dict2[key]);
            }
        }

        public static void AssertParameters(Dictionary<string, object> dict, Dictionary<string, object> dict2)
        {
            Assert.AreEqual(dict.Count, dict2.Count);
            foreach (string key in dict.Keys)
            {
                Assert.IsTrue(dict2.ContainsKey(key), $"The parameters does not contain key '{key}'");
                Assert.IsTrue(dict[key].Equals(dict2[key]), $"The expected value is {dict[key]}, the actual value is {dict2[key]}");
            }
        }

    }
}
