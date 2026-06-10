namespace ExpressionToSqlWhereClause.Test.EntityConfigToWhereClause;

[TestClass]
public class ToWhereClauseDemo_alias
{
    [TestMethod]
    public void 别名_基本使用_映射()
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
    public void 别名_Sql内置系统关键字()
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
    public void 别名_字段名称添加对象()
    {
        Expression<Func<Student, bool>> expOr = x => x.IsDel == true;

        Dictionary<string, string> dict = new Dictionary<string, string>()
        {
            { "IsDel", "b.IsDel" }
        };
        var searchCondition = expOr.ToWhereClause(dict);

        Assert.AreEqual(searchCondition.WhereClause, "b.IsDel = @IsDel");

        var para = new Dictionary<string, object>()
        {
            {"@IsDel", true},
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, para);
    }

    [TestMethod]
    public void 别名_表达式对比A()
    {
        Expression<Func<Student, bool>> expOr = a => a.Id == 1 && a.IsDel == true;

        var dict = new Dictionary<string, string>()
        {
            { "Id", "RouteId" }
        };
        var searchCondition = expOr.ToWhereClause(dict);

        Assert.AreEqual(searchCondition.WhereClause, "RouteId = @Id And IsDel = @IsDel");

        var para = new Dictionary<string, object>()
        {
            {"@Id", 1},
            {"@IsDel", true},
        };
        CollectionAssert.AreEqual(searchCondition.Parameters, para);
    }

    [TestMethod]
    public void 别名_表达式对比B()
    {
        Expression<Func<Student, bool>> expOr = a => a.Id == 1 && a.IsDel;

        var dict = new Dictionary<string, string>()
        {
            { "Id", "RouteId" },
        };
        var searchCondition = expOr.ToWhereClause(dict);

        Assert.AreEqual(searchCondition.WhereClause, "RouteId = @Id And IsDel = @IsDel");

        var para = new Dictionary<string, object>()
        {
            {"@Id", 1},
            {"@IsDel", true},
        };
        CollectionAssert.AreEqual(searchCondition.Parameters, para);
    }
}