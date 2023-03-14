using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause
{

    [TestClass]
    public class ExpressionDemo_值测试
    {
        [TestMethod]
        public void eq_string为null值()
        {
            Expression<Func<Student, bool>> expOr = a => a.Url == null;
            var searchCondition = expOr.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "Url Is Null");

            var para = new Dictionary<string, object>();
            CollectionAssert.AreEqual(searchCondition.Parameters, para);
        }

        [TestMethod]
        public void neq_string为null值()
        {
            Expression<Func<Student, bool>> expOr = a => a.Url != null;
            var searchCondition = expOr.ToWhereClause();
            Assert.AreEqual(searchCondition.WhereClause, "Url Is Not Null");

            var para = new Dictionary<string, object>();
            CollectionAssert.AreEqual(searchCondition.Parameters, para);

        }
    }
}