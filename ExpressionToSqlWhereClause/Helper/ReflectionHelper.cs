using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ExpressionToSqlWhereClause.Helper
{
    internal static class ReflectionHelper
    {
        #region GetProperties

        public static PropertyInfo[] GetProperties<T>() => typeof(T).GetProperties();

        public static PropertyInfo[] GetProperties(Type type) => type.GetProperties();

        public static PropertyInfo[] GetProperties<T>(BindingFlags bindingAttr) => typeof(T).GetProperties(bindingAttr);

        public static PropertyInfo[] GetProperties(Type type, BindingFlags bindingAttr) => type.GetProperties(bindingAttr);

        #endregion


        #region GetAttributeForProperty

        /// <summary>
        /// netstand2.0无法支持数据注解
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="propertyInfo"></param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute[] GetAttributeForProperty<TAttribute>(PropertyInfo propertyInfo, bool inherit)
        {
            var attributeType = typeof(TAttribute);
            object[] attrs = propertyInfo.GetCustomAttributes(attributeType, inherit);
            return attrs as TAttribute[];
        }
        #endregion

        #region GetAttributeForMethod
        public static object[] GetAttributeForMethod(MemberInfo method, Type attributeType)
        {
            if (method is null) throw new ArgumentNullException(nameof(method));
            object[] attrs = method.GetCustomAttributes(attributeType, true);
            if (attrs.Length == 1)
            {
                return attrs;
                //list.Add(attrs);
            }
            return attrs; //Length <= 0
        }

        public static TAttribute[] GetAttributeForMethod<TAttribute>(MemberInfo method) => GetAttributeForMethod(method, typeof(TAttribute)) as TAttribute[];

        #endregion
    }
}
