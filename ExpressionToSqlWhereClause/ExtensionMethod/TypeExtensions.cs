using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ExpressionToSqlWhereClause.ExtensionMethod
{
    internal static class TypeExtensions
    {

        public static IList MakeList(this Type t, params object[] items)
        {
            Type type = typeof(List<>).MakeGenericType(t);

            object list = Activator.CreateInstance(type);
            IList ilist = list as IList;
            if (ilist != null)
            {
                foreach (object o in items)
                {
                    ilist.Add(o);
                }
            }
            return ilist;
        }

        #region IsObjectCollection

        /// <summary>
        /// 是否为列表对象
        /// </summary>
        /// <param name="type"></param>
        /// <param name="objectType">具体的类型</param>
        /// <param name="objectTypeEqual">objectType应该是</param>
        /// <returns></returns>
        public static bool IsObjectCollection(this Type type, out Type objectType, Type objectTypeEqual)
        {
            if (type == typeof(string))
            {
                objectType = typeof(string);
                if (objectType == objectTypeEqual)
                {
                    return true;
                }

                return false; //string 类型也是 IEnumerable 对象
            }

            //Array : ICloneable, IList, ICollection, IEnumerable, IStructuralComparable, IStructuralEquatable
            //List<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>

            //给定一个Type对象，最简单的方法是测试以查看它是否实际上是一个对象列表？即Array或IEnumerable / IEnumerable&%lt&gt;
            var isObjectList = typeof(IEnumerable).IsAssignableFrom(type);
            objectType = type.GetElementType(); //给 Array 用的
            if (objectType == null)
            {
                if (type.GenericTypeArguments.Length == 1) //给List<T> 用的
                {
                    objectType = type.GenericTypeArguments[0];
                }
            }

            if (objectType != null && objectTypeEqual != null)
            {
                var isTargetType = objectType.IsSubClassOfRawGeneric(objectTypeEqual);
                return isTargetType;
            }
            else
            {
                return isObjectList;
            }

        }

        public static bool IsObjectCollection(this Type type) => type.IsObjectCollection(out _, null);

        public static bool IsObjectCollection(this Type type, out Type objectType) =>
            type.IsObjectCollection(out objectType, null);

        public static bool IsObjectCollection(this Type type, Type objectTypeEqual) =>
            type.IsObjectCollection(out _, objectTypeEqual);

        #endregion

        public static bool IsSubClassOfRawGeneric(this Type type, Type generic)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));
            if (generic == null) throw new ArgumentNullException(nameof(generic));

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
        /// 返回Nullable&lt;T&gt;里的T类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns>返回Nullable&lt;T&gt;里的T类型,T是枚举值</returns>
        public static Type GetNullableTType(this Type type)
        {
            return type.GetProperty("Value").PropertyType;
        }

        public static bool IsNullableType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static bool IsStructType(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            var isStructType = !type.IsClass;
            if (isStructType)
            {
                return true;
            }

            if (type.IsNullableType() && !type.GetNullableTType().IsClass)
            {
                return true;
            }
            return false;
        }

    }
}