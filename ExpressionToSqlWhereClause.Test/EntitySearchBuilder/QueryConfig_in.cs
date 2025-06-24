using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Infra.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_in
{
    [TestMethod]
    public void @in()
    {
        var searchModel = new Model_in()
        {
            Id = "1,2",
            Sex = "1",
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));

        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.Id),
            nameof(searchModel.Sex),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id1, @Id2) And Sex In (@Sex1)");
        var dict = new Dictionary<string, object>
        {
            { "@Id", searchModel.Id},
            { "@Id1", 1}, // Model_People.Id 的类型是int
            { "@Id2", 2}, // Model_People.Id 的类型是int
            { "@Sex", searchModel.Sex}, // Model_People.Id 的类型是int
            { "@Sex1", (sbyte)1 }
        };

        DictionaryAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void @in2()
    {
        var searchModel = new Model_in2()
        {
            Id = 1
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));

        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.Id),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id1)");
        var dict = new Dictionary<string, object>
        {
            { "@Id", "1"},//in 可以有多个值,所以这个值就是stirng类型的
            { "@Id1",1}, // Model_People.Id 的类型是int
        };

        DictionaryAssert.AreEqual(searchCondition.Parameters, dict);
    }
}