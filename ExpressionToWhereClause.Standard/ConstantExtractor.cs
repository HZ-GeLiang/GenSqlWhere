using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ExpressionToWhereClause.Standard
{
    public static class ConstantExtractor
    {

        /// <summary>
        /// 这个方法我自己写的
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isParse">是否解析</param>
        /// <returns></returns>
        public static string ConstantExpressionValueToString(object value, out bool isParse)
        {
            IEnumerable loopObj = null;
            if (value.GetType().FullName.StartsWith("System.Linq.Enumerable+SelectEnumerableIterator`2") && value is IEnumerable)
            {
                loopObj = (IEnumerable)value;
            }

            isParse = loopObj != null;

            if (!isParse)
            {
                return null;
            }

            object firstValue = null;

            StringBuilder sb = new StringBuilder();
            var index = -1;
            foreach (var obj in loopObj) //这个obj的类型就上面的T2
            {
                index++;
                if (index == 0)
                {
                    firstValue = obj;
                }

                sb.Append(obj).Append(",");
            }

            if (index < 0)
            {
                return string.Empty;
            }
            else if (index == 0)
            {
                return firstValue.ToString();
            }
            else
            {
                var intxt = sb.Remove(sb.Length - 1, 1).ToString(); //RemoveLastChar
                return intxt;
            }

        }


        /// <summary>
        /// 获得Expression的值
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static object ParseConstant(Expression expression)
        {
            if (expression is ConstantExpression constantExpression)
            {
                return ParseConstantExpression(constantExpression);
            }
            else if (expression is MemberExpression memberExpression)
            {
                return ParseMemberConstantExpression(memberExpression);
            }
            else if (expression is MethodCallExpression methodCallExpression)
            {
                return ParseMethodCallConstantExpression(methodCallExpression);
            }
            else if (expression is ConditionalExpression conditionalExpression)
            {
                return ParseConditionalExpression(conditionalExpression);
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                if (expression.GetType().Name == "MethodBinaryExpression")
                {
                    return ParseMethodBinaryExpression(binaryExpression);
                }
                else if (binaryExpression.GetType().Name == "SimpleBinaryExpression")
                {
                    return ParseSimpleBinaryExpression(binaryExpression);
                }
                else if (binaryExpression.GetType().Name == "LogicalBinaryExpression")
                {
                    return ParseConstant(binaryExpression.Right);
                }
                else
                {
                    throw new NotSupportedException($"Unknow expression {expression.GetType()}");
                }

            }
            else if (expression is UnaryExpression convertExpression
                && expression.NodeType == ExpressionType.Convert)
            {
                return ParseConvertExpression(convertExpression);
            }
            else
            {
                throw new NotSupportedException($"Unknow expression {expression.GetType()}");
            }
        }

        private static object ParseConstantExpression(ConstantExpression constantExpression)
        {
            //这里不使用 return ConstantExtractor.ConstantExpressionValueToString(constantExpression.Value,out var _), 由外层去使用
            return constantExpression.Value;
        }

        /// <summary>
        /// for example: get the age value from u.Age
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        private static object ParseMemberConstantExpression(MemberExpression memberExpression)
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
                int a = 33;
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
                    return propertyInfo.GetValue(value);
                default:
                    throw new NotSupportedException($"Unknow Member type {memberExpression.Member.MemberType}");
            }
        }

        /// <summary>
        /// For example: execute the method to get the value, like: u.Name.SubString(1,2), call the 'SubString' mehod
        /// </summary>
        /// <param name="methodCallExpression"></param>
        /// <returns></returns>
        private static object ParseMethodCallConstantExpression(MethodCallExpression methodCallExpression)
        {
            MethodInfo mi = methodCallExpression.Method;
            object instance = null;
            object[] parameters = null;
            if (methodCallExpression.Object != null)
            {
                instance = ParseConstant(methodCallExpression.Object);
            }
            if (methodCallExpression.Arguments != null && methodCallExpression.Arguments.Count > 0)
            {
                parameters = new object[methodCallExpression.Arguments.Count];
                for (int i = 0; i < methodCallExpression.Arguments.Count; i++)
                {
                    Expression expression = methodCallExpression.Arguments[i];
                    parameters[i] = ParseConstant(expression);
                }
            }

            return mi.Invoke(instance, parameters);
        }

        private static object ParseConditionalExpression(ConditionalExpression conditionalExpression)
        {
            bool condition = (bool)ParseConstant(conditionalExpression.Test);
            if (condition)
            {
                return ParseConstant(conditionalExpression.IfTrue);
            }
            else
            {
                return ParseConstant(conditionalExpression.IfFalse);
            }
        }

        private static object ParseMethodBinaryExpression(BinaryExpression methodBinaryExpression)
        {
            object left = ParseConstant(methodBinaryExpression.Left);
            object right = ParseConstant(methodBinaryExpression.Right);
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
            else
            {
                return new NotSupportedException();
            }
        }

        private static object ParseConvertExpression(UnaryExpression convertExpression)
        {
            object value = ParseConstant(convertExpression.Operand);
            return Convert.ChangeType(value, convertExpression.Type);
        }
    }
}
