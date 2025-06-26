using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Infra.ExtensionMethods;
using Infra.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_contains
{
    [TestMethod]
    public void list_contains_变量在链式编程中()
    {
        //#issue 2025.6.19
        //GetList().Select(a => (int?)a).ToList() 代码中 .Select(a => (int?)a) 在编译后会变成如下代码
        //.Select((Func<int, int?>)((int num) => num)) 然后会造成不支持的 表达式解析

        var expression =
            default(Expression<Func<Model_contians, bool>>)
            .WhereIf(true, a => GetList().Select(a => (int?)a).ToList().Contains(a.userid))
            ;

        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "userid In (@userid1 , @userid2 , @userid3 )");
        Dictionary<string, object> expectedParameters = new()
        {
            { "@userid1", 1 },
            { "@userid2", 2 },
            { "@userid3", 3 },
        };
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    [TestMethod]
    public void list_contains_提前准备一个变量()
    {
        var ids = GetList().Select(a => (int?)a).ToList();
        var expression =
            default(Expression<Func<Model_contians, bool>>)
            .WhereIf(true, a => ids.Contains(a.userid))
            ;

        var searchCondition = expression.ToWhereClause();
        Assert.AreEqual(searchCondition.WhereClause, "userid In (@userid1 , @userid2 , @userid3 )");

        Dictionary<string, object> expectedParameters = new()
        {
            { "@userid1", 1 },
            { "@userid2", 2 },
            { "@userid3", 3 },
        };
        DictionaryAssert.AreEqual(expectedParameters, searchCondition.Parameters);
    }

    public static List<int> GetList()
    {
        return new List<int>() { 1, 2, 3 };
    }
}