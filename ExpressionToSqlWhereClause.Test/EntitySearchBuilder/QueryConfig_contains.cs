using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Infra.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;
using ExpressionToSqlWhereClause.ExtensionMethods;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_contains
{
    [TestMethod]
    public void list_contains_2()
    {
        //#issue 2025.6.19
        //GetList().Select(a => (int?)a).ToList() 代码中 .Select(a => (int?)a) 在编译后会变成如下代码
        //.Select((Func<int, int?>)((int num) => num)) 然后会造成不支持的 表达式解析

        var ids = GetList().Select(a => (int?)a).ToList();
        var expression =
            default(Expression<Func<Model_contians, bool>>)
            .WhereIf(true, a => GetList().Select(a => (int?)a).ToList().Contains(a.userid))
            ;

        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "userid In (@userid)");
        Assert.AreEqual(searchCondition.Parameters["@userid"].ToString(), "1,2,3");
    }

    [TestMethod]
    public void list_contains_1()
    {
        var ids = GetList().Select(a => (int?)a).ToList();
        var expression =
            default(Expression<Func<Model_contians, bool>>)
            .WhereIf(true, a => ids.Contains(a.userid))
            ;

        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "userid In (@userid)");
        Assert.AreEqual(searchCondition.Parameters["@userid"].ToString(), "1,2,3");
    }

    public static List<int> GetList()
    {
        return new List<int>() { 1, 2, 3 };
    }
}