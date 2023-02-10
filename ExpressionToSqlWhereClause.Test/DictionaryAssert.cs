using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test
{
    public class DictionaryAssert
    {

        #region IsObjectCollection
        private static bool IsSubClassOfRawGeneric(Type type, Type generic)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            if (generic is null) throw new ArgumentNullException(nameof(generic));

            while (type != null && type != typeof(object))
            {
                var isTheRawGenericType = IsTheRawGenericType(type);
                if (isTheRawGenericType) return true;
                type = type.BaseType;
            }

            return false;

            bool IsTheRawGenericType(Type test)
                => generic == (test.IsGenericType ? test.GetGenericTypeDefinition() : test);
        }

        /// <summary>
        /// 是否为列表对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectType">具体的类型</param>
        /// <param name="objectTypeEqual">objectType应该是</param>
        /// <returns></returns>
        private static bool IsObjectCollection(Type type, out Type objectType, Type objectTypeEqual)
        {
            if (type == typeof(string))
            {
                objectType = typeof(string);
                if (objectType == objectTypeEqual)
                {
                    return true;
                }
                return false;//string 类型也是 IEnumerable 对象
            }

            //Array : ICloneable, IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
            //List<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>

            //给定一个Type对象，最简单的方法是测试以查看它是否实际上是一个对象列表？即Array或IEnumerable / IEnumerable&%lt&gt;
            var isObjectList = typeof(IEnumerable).IsAssignableFrom(type);
            objectType = type.GetElementType(); //给 Array 用的
            if (objectType is null)
            {
                if (type.GenericTypeArguments.Length == 1) //给List<T> 用的
                {
                    objectType = type.GenericTypeArguments[0];
                }
            }
            if (objectType != null && objectTypeEqual != null)
            {
                var isTargetType = IsSubClassOfRawGeneric(objectType, objectTypeEqual);
                return isTargetType;
            }
            else
            {
                return isObjectList;
            }

        }
        private static bool IsObjectCollection(Type type) => IsObjectCollection(type, out _, null);
        private static bool IsObjectCollection(Type type, out Type objectType) => IsObjectCollection(type, out objectType, null);
        private static bool IsObjectCollection(Type type, Type objectTypeEqual) => IsObjectCollection(type, out _, objectTypeEqual);


        #endregion

        public static void AreEqual<TKey, TValue>(Dictionary<TKey, TValue> dict, Dictionary<TKey, TValue> dict2)
        {
            if (dict2 == null)
            {
                Assert.AreEqual(dict, null);
                return;
            }
            if (dict == null)
            {
                Assert.Fail(nameof(dict) + "不能为空");
                return;
            }

            Assert.AreEqual(dict.Count, dict2.Count);


            CollectionAssert.AreEqual(dict.Keys, dict2.Keys);    //判断key

            //判断value

            foreach (var key in dict.Keys)
            {
                Assert.IsTrue(dict2.ContainsKey(key), $"The parameters does not contain key '{key}'");
                if (IsObjectCollection(dict[key].GetType()))
                {
                    CollectionAssert.AreEqual(dict[key] as ICollection, dict2[key] as ICollection);
                }
                else
                {
                    Assert.IsTrue(dict[key].Equals(dict2[key]),
                        $"The expected value is {dict[key]} ({dict[key].GetType().FullName}), the actual value is {dict2[key]} ({dict2[key].GetType().FullName})");
                }
            }
        }

    }
}
