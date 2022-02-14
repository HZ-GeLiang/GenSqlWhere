using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EntityToSqlWhereClauseConfig.Helper
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
    }
}
