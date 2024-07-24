using System;
using System.Reflection;

namespace ExpressionToSqlWhereClause.Helper
{
    internal sealed class ReflectionHelper
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

        #region GetMethod

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="bindingAttr">默认与 GetMethod 方法的 BindingFlags 参数值 一致 </param>
        /// <returns></returns>
        public static MethodInfo? GetMethod(Type type, string methodName,
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
        {
            MethodInfo? method = type.GetMethod(methodName, bindingAttr);
            return method;
        }

        #endregion
    }
}