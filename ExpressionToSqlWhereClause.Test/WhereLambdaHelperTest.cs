using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause
{
    class peo
    {
        public string Name { get; set; }
    }

    [TestClass]
    public class WhereLambdaHelperTest
    {
        [TestMethod]
        public void GetExpression_HasValue()
        {
            Expression<Func<peo, bool>> exp = a => (a.Name ?? "") != "";
            var exp2 = WhereLambdaHelper.GetExpression_HasValue<peo>("Name");

            Assert.AreEqual(true, exp.ToString() == exp2.ToString());
        }
    }
}