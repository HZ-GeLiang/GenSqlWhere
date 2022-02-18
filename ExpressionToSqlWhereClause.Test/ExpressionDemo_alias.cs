using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EntityToSqlWhereClauseConfig.Helper;
using ExpressionToSqlWhereClause;
using ExpressionToSqlWhereClause.Test.ExtensionMethod;
using ExpressionToSqlWhereClause.Test.InputOrModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test
{
    [TestClass]
    public class ExpressionDemo_alias
    {

        [TestMethod]
        public void 别名()
        {
            Expression<Func<Student_Alias, bool>> expOr = a => a.Id == 1 || a.Id == 2;

            var dict = new Dictionary<string, string>()
            {
                { "Id", "RouteId" }
            };
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause(alias: dict);

            Assert.AreEqual(WhereClause, "(((RouteId = @Id)) Or ((RouteId = @Id1)))");

            var para = new Dictionary<string, object>()
            {
                {"@Id", 1},
                {"@Id1", 2},
            };
            CollectionAssert.AreEqual(Parameters, para);
        }

        [TestMethod]
        public void 别名2()
        {
            Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; }); // 和  () => { return x => x.IsDel; } 不一样

            var dict = new Dictionary<string, string>()
            {
                { "Id", "RouteId" }
            };
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause(alias: dict);

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
        public void 别名3()
        {
            Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
            expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; }); // 和  () => { return x => x.IsDel; } 不一样

            var dict = new Dictionary<string, string>()
            {
                { "Id", "RouteId" },
                { "IsDel", "b.IsDel" }
            };
            (string WhereClause, Dictionary<string, object> Parameters) = expOr.ToWhereClause(alias: dict);

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
