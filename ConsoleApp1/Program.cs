using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 要解析的表达式字符串
            string exprString = "x => x * x";

            // 使用 DynamicExpressionParser.ParseLambda 方法解析表达式字符串
            LambdaExpression lambdaExpr = DynamicExpressionParser.ParseLambda(typeof(int), typeof(int), exprString);

            // 输出解析出的表达式树
            Console.WriteLine(lambdaExpr);

            // 编译表达式并调用它
            var square = (int)lambdaExpr.Compile().DynamicInvoke(5);
            Console.WriteLine(square);
        }
    }
}