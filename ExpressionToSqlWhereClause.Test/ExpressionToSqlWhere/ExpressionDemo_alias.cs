using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause;

[TestClass]
public class ExpressionDemo_alias
{
    [TestMethod]
    public void Sql内置系统关键字()
    {
        Expression<Func<Student_mssql_buildIn_name, bool>> expOr = a => a.Index == 1 || a.Index == 2;

        var dict = new Dictionary<string, string>()
        {
            { "Index", "[Index]" }
        };

        var searchCondition = expOr.ToWhereClause(dict);

        Assert.AreEqual(searchCondition.WhereClause, "[Index] = @Index Or [Index] = @Index1");

        var para = new Dictionary<string, object>()
        {
            {"@Index", 1},
            {"@Index1", 2},
        };
        CollectionAssert.AreEqual(searchCondition.Parameters, para);
    }

    [TestMethod]
    public void 别名()
    {
        Expression<Func<Student_Alias, bool>> expOr = a => a.Id == 1 || a.Id == 2;

        var dict = new Dictionary<string, string>()
        {
            { "Id", "RouteId" }
        };
        var searchCondition = expOr.ToWhereClause(dict);

        Assert.AreEqual(searchCondition.WhereClause, "RouteId = @Id Or RouteId = @Id1");

        var para = new Dictionary<string, object>()
        {
            {"@Id", 1},
            {"@Id1", 2},
        };
        CollectionAssert.AreEqual(searchCondition.Parameters, para);
    }

    [TestMethod]
    public void 别名2()
    {
        Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
        expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; });  //                         ((IsDel = @IsDel)))
        //                    和  () => { return x => x.IsDel; } 不一样:解析不一样,导致最后的sql 语句不一样,  (IsDel = @IsDel)

        var dict = new Dictionary<string, string>()
        {
            { "Id", "RouteId" }
        };
        var searchCondition = expOr.ToWhereClause(dict);

        Assert.AreEqual(searchCondition.WhereClause, "(RouteId = @Id Or RouteId = @Id1) And IsDel = @IsDel");

        var para = new Dictionary<string, object>()
        {
            {"@Id", 1},
            {"@Id1", 2},
            {"@IsDel", true},
        };
        CollectionAssert.AreEqual(searchCondition.Parameters, para);
    }

    [TestMethod]
    public void 别名3()
    {
        Expression<Func<Student, bool>> expOr = a => a.Id == 1 || a.Id == 2;
        expOr = expOr.AndIf(true, () => { return x => x.IsDel == true; });  //                         ((IsDel = @IsDel)))
        //                    和  () => { return x => x.IsDel; } 不一样:解析不一样,导致最后的sql 语句不一样,  (IsDel = @IsDel)

        var dict = new Dictionary<string, string>()
        {
            { "Id", "RouteId" },
            { "IsDel", "b.IsDel" }
        };
        var searchCondition = expOr.ToWhereClause(dict);

        Assert.AreEqual(searchCondition.WhereClause, "(RouteId = @Id Or RouteId = @Id1) And b.IsDel = @IsDel");

        var para = new Dictionary<string, object>()
        {
            {"@Id", 1},
            {"@Id1", 2},
            {"@IsDel", true},
        };
        CollectionAssert.AreEqual(searchCondition.Parameters, para);
    }
}