using System.Linq.Expressions;
using System.Reflection;

namespace ExpressionToSqlWhereClause.Helper
{
    internal static class ExpressionHelper
    {
        internal class Visitor : ExpressionVisitor
        {
            public object Value { get; set; }
            protected override Expression VisitMember(MemberExpression member)
            {
                if (member.Expression is ConstantExpression && member.Member is FieldInfo)
                {
                    object container = ((ConstantExpression)member.Expression).Value;
                    object value = ((FieldInfo)member.Member).GetValue(container);
                    this.Value = value;
                    //Console.WriteLine("Got value: {0}", value);
                }
                return base.VisitMember(member);
            }
        }
        public static string GetLeftMemberName(dynamic left)
        {
            string leftMemberName;
            if (left is System.Linq.Expressions.UnaryExpression)
            {
                leftMemberName = left.Operand.Member.Name as string;
            }
            else
            {
                leftMemberName = left.Member.Name as string;
            }

            if (string.IsNullOrEmpty(leftMemberName))
            {
                throw new ExpressionToSqlWhereClauseException("解析表达式的代码有问题,请修改程序");
            }
            return leftMemberName;
        }

        public static dynamic GetRightValue(dynamic right)
        {
            if (right is System.Linq.Expressions.UnaryExpression)
            {
                var visitor = new Visitor();
                visitor.Visit(right.Operand);
                dynamic rightValue = visitor.Value;
                return rightValue;
            }

            if (right is System.Linq.Expressions.ConstantExpression)
            {
                dynamic rightValue = right.Value;
                return rightValue;
            }

            // 调试时有这个类型, 但是代码没法这样写
            //if (right is System.Linq.Expressions.FieldExpression)
            //{
            //}
            //下面这个if假设是 right is System.Linq.Expressions.FieldExpression 的情况
            if (right.NodeType == System.Linq.Expressions.ExpressionType.MemberAccess &&
                right.Expression.NodeType == System.Linq.Expressions.ExpressionType.Constant)
            {
                //官方获得值的方法: https://docs.microsoft.com/zh-cn/dotnet/api/system.linq.expressions.expression.field?redirectedfrom=MSDN&view=netframework-4.8
                dynamic rightValue = Expression.Lambda(right).Compile()();
                return rightValue;
            }

            #region 
            //如果要解析其他的 Expression
            //1.  在 Nuget:ExpressionToWhereClause 这个项目有  ConstantExtractor  文件 ,可以去参考看看 

            //2. 在 https://github.com/stromblom/ExpressionToWhere 这个项目里看到的代码,记录下
            //BinaryExpression node
            //object value = Expression.Lambda<Func<object>>(Expression.Convert(node.Right, typeof(object))).Compile().Invoke();
            //object value = Expression.Lambda<Func<object>>(Expression.Convert(right, typeof(object))).Compile().Invoke(); 

            //object value = Expression.Lambda<Func<object>>(Expression.Convert(node, typeof(object))).Compile().Invoke();
            #endregion

            throw new ExpressionToSqlWhereClauseException("解析表达式的代码有问题,请修改程序");
        }
    }
}
