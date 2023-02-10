using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause
{

    [TestClass]
    public class ExpressionDemo_值测试
    {
        [TestMethod]
        public void eq_string为null值()
        {

            Expression<Func<Student, bool>> expOr = a => a.Url == null;
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause();
            Assert.AreEqual(WhereClause, "Url Is Null");

            var para = new Dictionary<string, object>();
            CollectionAssert.AreEqual(Parameters, para);
        }

        [TestMethod]
        public void neq_string为null值()
        {

            Expression<Func<Student, bool>> expOr = a => a.Url != null;
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause();
            Assert.AreEqual(WhereClause, "Url Is Not Null");

            var para = new Dictionary<string, object>();
            CollectionAssert.AreEqual(Parameters, para);

        }
    }
}