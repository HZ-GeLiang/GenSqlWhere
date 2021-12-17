using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test
{
    public class DictionaryAssert
    {
        public static  void AssertParameters(Dictionary<string, object> dict, Dictionary<string, object> dict2)
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
