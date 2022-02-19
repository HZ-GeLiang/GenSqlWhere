using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
 
namespace EntityToSqlWhereClauseConfig.Helper
{

    internal class ConvertHelper
    {
        public static object ChangeType(object val, Type type)
        {
            bool IsNullableType(Type type) => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

            Type GetNullableTType(Type type) => type.GetProperty("Value").PropertyType;

            return Convert.ChangeType(val, IsNullableType(type) ? GetNullableTType(type) : type);
        }
         
    }
}
