using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressionToWhereClause.Standard;

namespace PredicateBuilder.Standard.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //Class1.Test();
            var whereLambda = Test.GetWhereLambda();
            List<Expression<Func<Route, bool>>> listExp = whereLambda.ToExpressionList();
            Expression<Func<Route, bool>> exp = whereLambda.ToExpression();
            //�ȼ�
            //List<Expression<Func<Route, bool>>> listExp = whereLambda;
            //Expression<Func<Route, bool>> exp = whereLambda;

            var sql = exp.ToWhereClause();

            Console.WriteLine("Hello World!");
        }
    }
}
