using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressionToWhereClause.Standard;

namespace PredicateBuilder.Standard.Test
{
    [TestClass]
    public class GenSqlWhereDemo
    {
        [TestMethod]
        public void GenSqlWhere()
        {
            //Class1.Test();
            var whereLambda = TestData.GetWhereLambda();
            //List<Expression<Func<Route, bool>>> listExp = whereLambda.ToExpressionList(); //可以给ef用
            Expression<Func<People, bool>> exp = whereLambda.ToExpression();
            //等价
            //List<Expression<Func<Route, bool>>> listExp = whereLambda;
            //Expression<Func<Route, bool>> exp = whereLambda;

            (string WhereClause, Dictionary<string, object> Parameters) result = exp.ToWhereClause();

            var sql = result.WhereClause;
            var param = result.Parameters;

            Assert.AreEqual(sql, "Id In @Id And Sex In @Sex And IsDel = @IsDel And DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1 And Url Like @Url");


            Console.WriteLine("Hello World!");
        }

        [TestMethod]
        public void GenSqlWhere2()
        {
            var whereLambda = TestData.GetWhereLambda2();
            (string WhereClause, Dictionary<string, object> Parameters) result = whereLambda.ToExpression().ToWhereClause();

            var sql = result.WhereClause;
            var param = result.Parameters;

             Assert.AreEqual(sql, "DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1");


            Console.WriteLine("Hello World!");
        }


    }
}
