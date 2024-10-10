using System;
using System.Reflection;

namespace ExpressionToSqlWhereClause.Helpers;

internal sealed class ConvertHelper
{
    public static object ChangeType(object val, Type type)
    {
        bool IsNullableType(Type type) => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        Type GetNullableTType(Type type) => type.GetProperty("Value").PropertyType;

        return Convert.ChangeType(val, IsNullableType(type) ? GetNullableTType(type) : type);
    }
}