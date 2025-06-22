using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
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

        var whereLambda = new QueryConfig<Model_People, Model_in>(searchModel);

        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.Id),
            nameof(searchModel.Sex),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id) And Sex In (@Sex)");
        var dict = new Dictionary<string, object>
        {
            { "@Id", searchModel.Id},
            { "@Sex", searchModel.Sex}
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void @in2()
    {
        var searchModel = new Model_in2()
        {
            Id = 1
        };

        var whereLambda = new QueryConfig<Model_People, Model_in2>(searchModel);

        whereLambda[SearchType.In] = new List<string>
        {
            nameof(searchModel.Id),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id In (@Id)");
        var dict = new Dictionary<string, object>
        {
            { "@Id", "1"},//in 可以有多个值,所以这个值就是stirng类型的
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }
}