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
    public void @in_stirng_type()
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

        Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id1 , @Id2 ) And Sex In (@Sex1 )");
        var dict = new Dictionary<string, object>
        {
            //{ "@Id", searchModel.Id}, //移除未用到的变量
            { "@Id1", 1}, // Model_People.Id 的类型是int
            { "@Id2", 2}, // Model_People.Id 的类型是int
            //{ "@Sex", searchModel.Sex}, //移除未用到的变量
            { "@Sex1", (sbyte)1 }  // Model_People.Sex 的类型是 sbyte
        };

        DictionaryAssert.AreEqual(searchCondition.Parameters, dict);

        Assert.AreEqual(searchCondition.Parameters["@Sex1"].GetType(), typeof(sbyte));
    }

    [TestMethod]
    public void @in_nullable_type()
    {
        var searchModel = new Model_in2()
        {
            Id = 1 //nullable_type
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));

        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.Id),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id1 )");
        var dict = new Dictionary<string, object>
        {
            //{ "@Id", "1"},//in 可以有多个值,所以这个值就是stirng类型的
            { "@Id1",1}, // Model_People.Id 的类型是int
        };

        DictionaryAssert.AreEqual(searchCondition.Parameters, dict);
    }
}