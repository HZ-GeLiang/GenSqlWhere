using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    public class DictionaryAssert
    {
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
    }
}
