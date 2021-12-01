using System;
using System.Reflection;

namespace EntityToSqlWhereClauseConfig.ExtensionMethod
{
    internal static class TypeExtensions
    {
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
