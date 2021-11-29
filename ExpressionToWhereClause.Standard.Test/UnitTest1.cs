using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EntityToSqlWhereCaluseConfig.Test;
using EntityToSqlWhereCaluseConfig.Test.ExtensionMethod;
using ExpressionToSqlWhereClause;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToWhereClause.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Expression<Func<Route, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel; });
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause();


            Assert.AreEqual(WhereClause, "((((Id = @Id)) Or ((Id = @Id1))) And (IsDel = @IsDel))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(Parameters, para);
        }

        [TestMethod]
        public void TestMethod2()
        {
            Expression<Func<Route, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; }); // 和  () => { return x => x.IsDel; } 不一样

            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause();

            Assert.AreEqual(WhereClause, "((((Id = @Id)) Or ((Id = @Id1))) And ((IsDel = @IsDel)))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(Parameters, para);

        }

        [TestMethod]
        public void 测试别名()
        {
            Expression<Func<Route, bool>> expOr = a => a.Id == 1 || a.Id == 2;

            var dict = new Dictionary<string, string>()
            {
                { "Id", "RouteId" }
            };
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause(dict);

            Assert.AreEqual(WhereClause, "(((RouteId = @Id)) Or ((RouteId = @Id1)))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
            };
            CollectionAssert.AreEqual(Parameters, para);
        }

        [TestMethod]
        public void 测试别名2()
        {
            Expression<Func<Route, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; }); // 和  () => { return x => x.IsDel; } 不一样

            var dict = new Dictionary<string, string>()
            {
                { "Id", "RouteId" }
            };
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause(dict);

            Assert.AreEqual(WhereClause, "((((RouteId = @Id)) Or ((RouteId = @Id1))) And ((IsDel = @IsDel)))");  

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(Parameters, para);
        }

        [TestMethod]
        public void 测试别名3()
        {
            Expression<Func<Route, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; }); // 和  () => { return x => x.IsDel; } 不一样

            var dict = new Dictionary<string, string>()
            {
                { "Id", "RouteId" },
                { "IsDel", "b.IsDel" }
            };
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause(dict);

            Assert.AreEqual(WhereClause, "((((RouteId = @Id)) Or ((RouteId = @Id1))) And ((b.IsDel = @IsDel)))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
                {"@IsDel", true},
            };
            CollectionAssert.AreEqual(Parameters, para);
        }
    }
}
