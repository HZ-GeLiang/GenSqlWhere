﻿using ExpressionToSqlWhereClause.Consts;
using ExpressionToSqlWhereClause.ExtensionMethods;
using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ExpressionToSqlWhereClause.ExpressionTree;

public static class ConstantExtractor
{
    /// <summary>
    /// 把IEnumerable的数据的值给显示出来,多个值之间用,分隔
    /// </summary>
    /// <param name="value"></param>
    /// <param name="isIEnumerableObj">是否是isIEnumerable 对象</param>
    /// <param name="hasNullItem">存在null值</param>
    /// <returns></returns>
    public static string ConstantExpressionValueToString(object value, out bool isIEnumerableObj, out bool hasNullItem)
    {
        hasNullItem = false;
        if (value == null)
        {
            isIEnumerableObj = false;
            return null;
        }

        if (!value.GetType().IsObjectCollection())
        {
            isIEnumerableObj = false;
            return null;
        }

        IEnumerable loopObj = (IEnumerable)value;
        isIEnumerableObj = true;
        object firstValue = null;

        var loopCount = -1; //  -1  0  1 只有3种值
        foreach (var obj in loopObj)
        {
            loopCount++;
            if (loopCount > 0)
            {
                break;
            }
            if (obj.GetType().IsStructType())
            {
                firstValue = obj;
            }
            else
            {
                firstValue = $"'{obj}'";
            }
        }
        if (loopCount < 0)
        {
            return string.Empty;
        }

        if (loopCount == 0)
        {
            return firstValue.ToString();//列表只有一个, 直接返回第一项
        }

        //else  loopCount > 0

        StringBuilder sb = new();
        foreach (var obj in loopObj)
        {
            if (obj == null)
            {
                hasNullItem = true; //需要在sql语句中生成 or 字段 is null
            }
            else
            {
                if (obj.GetType().IsStructType())
                {
                    sb.Append(obj).Append(",");
                }
                else
                {
                    sb.Append("'").Append(obj).Append("'").Append(",");
                }
            }
        }
        var txt = sb.Remove(sb.Length - 1, 1).ToString(); //RemoveLastChar
        return txt;
    }

    /// <summary>
    /// 获得Expression的值
    /// </summary>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static object ParseConstant(Expression expression)
    {
#if DEBUG
        StackFrame frame = new(1, true);
        var method = frame.GetMethod();
        var fileName = frame.GetFileName();
        var lineNumber = frame.GetFileLineNumber();
#endif

        if (expression is ConstantExpression constantExpression)
        {
            return ConstantExtractor.ParseConstantExpression(constantExpression);
        }
        else if (expression is MemberExpression memberExpression)
        {
            return ParseMemberConstantExpression(memberExpression);
        }
        else if (expression is MethodCallExpression methodCallExpression)
        {
            return ConstantExtractor.ParseMethodCallConstantExpression(methodCallExpression);
        }
        else if (expression is ConditionalExpression conditionalExpression)
        {
            return ConstantExtractor.ParseConditionalExpression(conditionalExpression);
        }
        else if (expression is BinaryExpression binaryExpression)
        {
            if (expression.GetType().Name == "MethodBinaryExpression")
            {
                return ConstantExtractor.ParseMethodBinaryExpression(binaryExpression);
            }
            else if (binaryExpression.GetType().Name == "SimpleBinaryExpression")
            {
                return ConstantExtractor.ParseSimpleBinaryExpression(binaryExpression);
            }
            else if (binaryExpression.GetType().Name == "LogicalBinaryExpression")
            {
                return ConstantExtractor.ParseConstant(binaryExpression.Right);
            }
            else
            {
                throw new NotSupportedException($"Unknow expression {expression.GetType()}");
            }
        }
        else if (expression is UnaryExpression convertExpression)
        {
            if (expression.NodeType == ExpressionType.Convert)
            {
                return ConstantExtractor.ParseConvertExpression(convertExpression);
            }
            else
            {
                throw new NotSupportedException($"Unknow expression {expression.GetType()}");
            }
        }
        else if (expression is System.Linq.Expressions.NewExpression newExpression)
        {
            if (expression.NodeType == ExpressionType.New)
            {
                return ConstantExtractor.ParseNewExpression(newExpression);
            }
            else
            {
                throw new NotSupportedException($"Unknow expression {expression.GetType()}");
            }
        }

        //已知的不支持的有:
        //if (expression.GetType().FullName == ExpressionFullNameSpaceConst.TypedParameter)
        //{
        //    throw new NotSupportedException($"Unknow expression {expression.GetType()}");
        //}



#if DEBUG

        System.Linq.Expressions.NewArrayExpression NewArrayExpression = expression as System.Linq.Expressions.NewArrayExpression;
#endif


        throw new NotSupportedException($"Unknow expression {expression.GetType()}");
    }

    public static object ParseConstantExpression(ConstantExpression constantExpression)
    {
        //这里不使用 return ConstantExtractor.ConstantExpressionValueToString(constantExpression.Value,out var _), 由外层去使用
        return constantExpression.Value;
    }

    /// <summary>
    /// for example: get the age value from u.Age
    /// </summary>
    /// <param name="memberExpression"></param>
    /// <returns></returns>
    public static object ParseMemberConstantExpression(MemberExpression memberExpression)
    {
        if (memberExpression.NodeType == ExpressionType.MemberAccess)
        {
            if (memberExpression.Member is PropertyInfo pi && pi.PropertyType == typeof(bool))
            {
                return true;
            }
        }

        if (memberExpression.NodeType == System.Linq.Expressions.ExpressionType.Constant)
        {
#if DEBUG
            System.Diagnostics.Debugger.Break(); //进来看看
#endif
            //官方获得值的方法: https://docs.microsoft.com/zh-cn/dotnet/api/system.linq.expressions.expression.field?redirectedfrom=MSDN&view=netframework-4.8
            //    dynamic rightValue = Expression.Lambda(memberExpression).Compile()();
            //    return rightValue;
        }

        // Firstly: Get the value of u
        object value = ParseConstant(memberExpression.Expression);

        //Secondly: get Age using reflect
        Type type = value.GetType();
        switch (memberExpression.Member.MemberType)
        {
            case MemberTypes.Field:
                FieldInfo fieldInfo = type.GetField(memberExpression.Member.Name);
                return fieldInfo.GetValue(value);

            case MemberTypes.Property:
                PropertyInfo propertyInfo = type.GetProperty(memberExpression.Member.Name);
                if (propertyInfo == null)
                {
                    //可空类型时遇到了, 详见 bug:#01
                    return value;
                }
                else
                {
                    return propertyInfo.GetValue(value);
                }

            default:
#if DEBUG
                StackFrame frame = new(1, true);
                var method = frame.GetMethod();
                var fileName = frame.GetFileName();
                var lineNumber = frame.GetFileLineNumber();
#endif
                throw new NotSupportedException($"Unknow Member type {memberExpression.Member.MemberType}");
        }
    }

    /// <summary>
    /// For example: execute the method to get the value,
    /// like: u.Name.SubString(1,2), call the 'SubString' mehod
    /// </summary>
    /// <param name="methodCallExpression"></param>
    /// <returns></returns>
    internal static object ParseMethodCallConstantExpression(MethodCallExpression methodCallExpression)
    //private static object ParseMethodCallConstantExpression(MethodCallExpression methodCallExpression)
    {
        MethodInfo mi = methodCallExpression.Method;
        object instance = null;
        object[] parameters = null;
        if (methodCallExpression.Object != null)
        {
            instance = ConstantExtractor.ParseConstant(methodCallExpression.Object);
        }
        if (methodCallExpression.Arguments != null && methodCallExpression.Arguments.Count > 0)
        {
            parameters = new object[methodCallExpression.Arguments.Count];
            for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
            {
                Expression expression = methodCallExpression.Arguments[i];

                if (expression.GetType().FullName == ExpressionFullNameSpaceConst.Property)
                {
                    var nodeType = (ExpressionType)(((dynamic)expression).Expression.NodeType);

                    if (nodeType == ExpressionType.Parameter)
                    {
                        return expression.ToString(); //u.CreateAt
                    }
                    else if (nodeType == ExpressionType.MemberAccess)
                    {
                        parameters[i] = ConstantExtractor.ParseConstant(expression);//GetInt(userFilter.Internal.Age);
                    }
                }
                else if (expression.GetType().FullName == ExpressionFullNameSpaceConst.TypedParameter)
                {
                    Debugger.Break();
                    throw new NotSupportedException($"Unknow expression {expression.GetType()}");
                }
                else
                {
                    parameters[i] = ConstantExtractor.ParseConstant(expression);
                }
            }
        }

        return mi.Invoke(instance, parameters);
    }

    private static object ParseConditionalExpression(ConditionalExpression conditionalExpression)
    {
        bool condition = (bool)ConstantExtractor.ParseConstant(conditionalExpression.Test);
        if (condition)
        {
            return ConstantExtractor.ParseConstant(conditionalExpression.IfTrue);
        }
        else
        {
            return ConstantExtractor.ParseConstant(conditionalExpression.IfFalse);
        }
    }

    private static object ParseMethodBinaryExpression(BinaryExpression methodBinaryExpression)
    {
        object left = ConstantExtractor.ParseConstant(methodBinaryExpression.Left);
        object right = ConstantExtractor.ParseConstant(methodBinaryExpression.Right);
        MethodInfo methodInfo = methodBinaryExpression.Method;
        if (methodInfo.IsStatic)
        {
            return methodInfo.Invoke(null, new object[] { left, right });
        }
        else
        {
            return methodInfo.Invoke(left, new object[] { right });
        }
    }

    private static object ParseSimpleBinaryExpression(BinaryExpression simpleBinaryExpression)
    {
        if (simpleBinaryExpression.NodeType == ExpressionType.ArrayIndex)
        {
            var array = ParseConstant(simpleBinaryExpression.Left) as Array;
            var index = (int)ParseConstant(simpleBinaryExpression.Right);
            return array.GetValue(index);
        }
        else if (simpleBinaryExpression.NodeType == ExpressionType.Coalesce)
        {
            var left = simpleBinaryExpression.Left;
            var right = simpleBinaryExpression.Right;

            // 编译表达式树为委托
            var leftFunc = Expression.Lambda(left).Compile();
            var rightFunc = Expression.Lambda(right).Compile();

            // 计算左右子表达式的值
            var leftValue = leftFunc.DynamicInvoke();
            if (leftValue == null)
            {
                var rightValue = rightFunc.DynamicInvoke();
                return rightValue;
            }
            else
            {
                return leftValue;
            }
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    private static object ParseConvertExpression(UnaryExpression convertExpression)
    {
        bool IsNullableType(Type type) => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);

        Type GetNullableTType(Type type) => type.GetProperty("Value").PropertyType;

        object ChangeType(object val, Type type) => Convert.ChangeType(val, IsNullableType(type) ? GetNullableTType(type) : type);

        object value = ConstantExtractor.ParseConstant(convertExpression.Operand);

        return ChangeType(value, convertExpression.Type);
    }

    private static object ParseNewExpression(NewExpression newExpression)
    {
        Type type = newExpression.Type;
        ConstructorInfo constructor = newExpression.Constructor;
        object[] arguments = new object[newExpression.Arguments.Count];

        // 解析参数列表
        for (int i = 0; i < newExpression.Arguments.Count; i++)
        {
            Expression argument = newExpression.Arguments[i];
            arguments[i] = GetValueFromExpression(argument);
        }

        // 创建新的对象
        object instance = constructor.Invoke(arguments);
        return instance;
    }

    private static object GetValueFromExpression(Expression expression)
    {
        if (expression is ConstantExpression constant)
        {
            return constant.Value;
        }
        else if (expression is MemberExpression member)
        {
            object instance = GetValueFromExpression(member.Expression);
            return member.Member is FieldInfo field ? field.GetValue(instance) : ((PropertyInfo)member.Member).GetValue(instance);
        }
        else if (expression is NewExpression newExpression)
        {
            Type type = newExpression.Type;
            ConstructorInfo constructor = newExpression.Constructor;
            object[] arguments = new object[newExpression.Arguments.Count];

            for (int i = 0; i < newExpression.Arguments.Count; i++)
            {
                Expression argument = newExpression.Arguments[i];
                arguments[i] = GetValueFromExpression(argument);
            }

            return constructor.Invoke(arguments);
        }

        throw new NotSupportedException("Unsupported expression type: " + expression.GetType().Name);
    }
}