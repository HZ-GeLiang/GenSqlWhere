using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_like
{
    [TestMethod]
    public void like()
    {
        var searchModel = new Model_like()
        {
            Url = "123",
        };
        var whereLambda = new QueryConfig<Model_People, Model_like>(searchModel);

        whereLambda[SearchType.Like] = new List<string>
        {
            nameof(searchModel.Url),
            nameof(searchModel.Data_Remark),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Url Like @Url");
        var dict = new Dictionary<string, object>
        {
            { "@Url", "%123%" }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void likeLeft()
    {
        var searchModel = new Input_likeLeft()
        {
            Url = "123",
        };
        var whereLambda = new QueryConfig<Model_People, Input_likeLeft>(searchModel);

        whereLambda[SearchType.LikeLeft] = new List<string>
        {
            nameof(searchModel.Url),
            nameof(searchModel.Data_Remark),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Url Like @Url");
        var dict = new Dictionary<string, object>
        {
            { "@Url", "123%" }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void likeRight()
    {
        var searchModel = new Input_likeRight()
        {
            Url = "123",
        };
        var whereLambda = new QueryConfig<Model_People, Input_likeRight>(searchModel);

        whereLambda[SearchType.LikeRight] = new List<string>
        {
            nameof(searchModel.Url),
            nameof(searchModel.Data_Remark),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Url Like @Url");
        var dict = new Dictionary<string, object>
        {
            { "@Url", "%123" }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }
}