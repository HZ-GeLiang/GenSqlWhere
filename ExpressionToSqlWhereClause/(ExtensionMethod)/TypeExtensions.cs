using System;
using System.Collections;

namespace ExpressionToSqlWhereClause
{

    internal static class TypeExtensions
    {
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

        public static bool IsObjectCollection(this Type type) => IsObjectCollection(type, out _, null);

        public static bool IsObjectCollection(this Type type, out Type objectType) =>
            IsObjectCollection(type, out objectType, null);

        public static bool IsObjectCollection(this Type type, Type objectTypeEqual) =>
            IsObjectCollection(type, out _, objectTypeEqual);

        #endregion


        //原文链接：https://blog.csdn.net/WPwalter/article/details/82859267
#if net45
        public static bool IsSubClassOfRawGeneric(this Type type, Type generic)
#else
        public static bool IsSubClassOfRawGeneric(this Type type, Type generic)
#endif
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

    }
}