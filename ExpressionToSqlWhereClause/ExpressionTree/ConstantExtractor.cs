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
    public static object ParseConstant(System.Linq.Expressions.Expression expression)
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
            return ConstantExtractor.ParseMemberConstantExpression(memberExpression);
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
        else if (expression is UnaryExpression unaryExpression)
        {
            if (expression.NodeType == ExpressionType.Convert)
            {
                var convertExpression = unaryExpression;
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
        else if (expression is System.Linq.Expressions.Expression linqExpression)
        {
            if (linqExpression.NodeType == ExpressionType.Lambda)
            {
                //-expression  { a => Convert(a, Nullable`1)}
                //System.Linq.Expressions.Expression { System.Linq.Expressions.Expression1<System.Func<int, int?>>}
                //#issue 2025.6.19

                throw new NotSupportedException($"Unsupported lambda expression body: {((LambdaExpression)linqExpression).Body.GetType()}");

                // 处理 Lambda 表达式，例如 a => Convert(a, Nullable<int>)
                //return ParseConstant(((LambdaExpression)linqExpression).Body);//调用自己(目前无法解析)
            }
            else if (linqExpression.NodeType == ExpressionType.Parameter)
            {
                //PrimitiveParameterExpression`1
                if (linqExpression.GetType().Name.StartsWith("PrimitiveParameterExpression`"))
                {
                    //var paramExpr = (ParameterExpression)linqExpression;

                    // 参数表达式无法直接求值，返回 null 或抛出异常
                    return null;
                }
            }
            else if (linqExpression.NodeType == ExpressionType.Convert)
            {
                // 处理 Convert 操作（如 Convert(a, Nullable<int>)）
                var unaryExpr = (UnaryExpression)linqExpression;
                return ParseConstant(unaryExpr.Operand); // 递归解析操作数
            }
            else if (linqExpression.NodeType == ExpressionType.Constant)
            {
                // 处理常量
                var constantExpr = (ConstantExpression)linqExpression;
                return constantExpr.Value;
            }

            throw new NotSupportedException($"Unknow expression {expression.GetType()}");
        }

#if DEBUG

        var fullname = expression.GetType().FullName;
        //已知的不支持的有:
        if (fullname == ExpressionFullNameSpaceConst.TypedParameter)
        {
            throw new NotSupportedException($"Unknow expression {expression.GetType()}");
        }

        var newArrayExpression = expression as System.Linq.Expressions.NewArrayExpression;
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
    {
        // 检查输入是否为 null
        if (methodCallExpression == null)
        {
            throw new ArgumentNullException(nameof(methodCallExpression));
        }

        MethodInfo methodInfo = methodCallExpression.Method;// 获取方法信息

        // 获取方法调用的对象（实例方法需要，非静态方法为 null）
        object target;

        if (methodInfo.IsStatic)
        {
            target = null;
        }
        else
        {
            // 解析调用对象（可能是 ConstantExpression 或其他表达式）
            if (methodCallExpression.Object == null)
            {
                throw new InvalidOperationException("Instance method call requires a target object.");
            }

            target = ConstantExtractor.ParseConstant(methodCallExpression.Object);

            //target = EvaluateExpression(methodCallExpression.Object);
        }

        if (methodCallExpression.Arguments == null)
        {
            return methodInfo.Invoke(target, null);
        }

        // 解析方法参数

        var argsCount = methodCallExpression.Arguments.Count;
        if (argsCount <= 1)
        {
            var i = 0;
            Expression expression = methodCallExpression.Arguments[i];
            return GetExpressionValue(target, methodInfo, expression, argsCount);
        }
        else
        {
            object[] arguments = new object[methodCallExpression.Arguments.Count];
            for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
            {
                Expression expression = methodCallExpression.Arguments[i];

                arguments[i] = GetExpressionValue(target, methodInfo, expression, argsCount);
            }
            return methodInfo.Invoke(target, arguments);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="target"></param>
    /// <param name="methodInfo"></param>
    /// <param name="expression"></param>
    /// <param name="argsCount">如果大于1, 那么返回的是 arguments[i] 的值 </param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    private static object GetExpressionValue(object target, MethodInfo methodInfo, Expression expression, int argsCount)
    {
        var fullName = expression.GetType().FullName;
        if (fullName == ExpressionFullNameSpaceConst.Property) //is 的方式无法通过
        {
            var nodeType = (ExpressionType)(((dynamic)expression).Expression.NodeType);

            if (nodeType == ExpressionType.Parameter)
            {
                return expression.ToString(); //u.CreateAt
            }

            if (nodeType == ExpressionType.MemberAccess)
            {
                var val = ConstantExtractor.ParseConstant(expression); //GetInt(userFilter.Internal.Age);
                if (argsCount > 1)
                {
                    return val;
                }
                else
                {
                    return methodInfo.Invoke(target, new object[1] { val }); // 使用反射调用方法并返回结果
                }
            }
        }
        else if (fullName == ExpressionFullNameSpaceConst.Field) //is 的方式无法通过
        {
            var val = ConstantExtractor.ParseConstant(expression);

            if (argsCount > 1)
            {
                return val;
            }
            else
            {
                return methodInfo.Invoke(target, new object[1] { val }); // 使用反射调用方法并返回结果
            }
        }
        else if (fullName == ExpressionFullNameSpaceConst.TypedParameter)
        {
#if DEBUG
            Debugger.Break();
#endif
            throw new NotSupportedException($"Unsupported lambda expression {expression.GetType()}");
        }
        else if (fullName == ExpressionFullNameSpaceConst.NewArrayInit) //is 的方式无法通过
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke(); //调用通过编译执行
        }
        else if (expression is ConstantExpression)
        {
            var val = ConstantExtractor.ParseConstant(expression);
            if (argsCount > 1)
            {
                return val;
            }
            else
            {
                return methodInfo.Invoke(target, new object[1] { val }); // 使用反射调用方法并返回结果
            }
        }
        else if (expression is MethodCallExpression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke(); //调用通过编译执行
        }
        throw new NotImplementedException($"GetExpressionValue: expression type: {expression.GetType()}");
    }

    // 辅助方法：递归求值表达式
    private static object EvaluateExpression(Expression expression)
    {
        if (expression == null)
        {
            return null;
        }

        // 处理 ConstantExpression
        if (expression is ConstantExpression constantExpression)
        {
            return constantExpression.Value;
        }

        // 处理 MethodCallExpression（递归调用）
        if (expression is MethodCallExpression methodCallExpression)
        {
            return ParseMethodCallConstantExpression(methodCallExpression);
        }

        // 处理其他类型的表达式（例如 MemberExpression）
        if (expression is MemberExpression memberExpression)
        {
            // 获取成员的持有者（可能是 ConstantExpression 或其他）
            object owner = EvaluateExpression(memberExpression.Expression);
            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                return propertyInfo.GetValue(owner);
            }
            if (memberExpression.Member is FieldInfo fieldInfo)
            {
                return fieldInfo.GetValue(owner);
            }
        }

        // 如果无法直接解析，尝试编译并执行表达式
        try
        {
            var lambda = Expression.Lambda(expression);
            return lambda.Compile().DynamicInvoke();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Cannot evaluate expression of type {expression.Type}.", ex);
        }
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

        var operand = convertExpression.Operand;
        var inputType = operand.Type; // 假设是 int
        var outputType = convertExpression.Type; // 假设是 int?

        if (operand.NodeType == ExpressionType.Parameter)
        {
            // 参数表达式无法直接求值
            // 示例: #issue 2025.6.19 , 遇到这种情况, 请修改代码
            throw new NotSupportedException("Parameter expression requires context of Select source.");
        }
        else
        {
            object value = ConstantExtractor.ParseConstant(operand);
            return ChangeType(value, outputType);
        }
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