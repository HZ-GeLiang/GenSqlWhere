using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EntityToSqlWhereClauseConfig.Test
{
    public class DictionaryAssert
    {
        public static void AreEqual<TKey, TValue>(Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> dict2)
        {
            Assert.AreEqual(dict.Count, dict2.Count);
            CollectionAssert.AreEqual(dict.Keys, dict2.Keys);
            foreach (var key in dict.Keys)
            {
                Assert.AreEqual(dict[key], dict2[key]);
            }
        }
    }
}
