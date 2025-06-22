using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Expressions;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_Demo2
{
    [TestMethod]
    public void Use_attr()
    {
        var time = DateTime.Parse("2021-8-8");
        var searchModel = new Input_Demo_Attr()
        {
            //Id = 1,
            Id = "1,2",
            Sex = "1",
            IsDel = true,
            Url = "123",
            DataCreatedAtStart = time.AddHours(-1),
            DataCreatedAtEnd = time,
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));

        Expression<Func<Model_People, bool>> exp = whereLambda.ToExpression();

        var searchCondition = exp.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel And Id In (@Id) And Sex In (@Sex) And DataCreatedAt >= @DataCreatedAt And DataCreatedAt < @DataCreatedAt1 And Url Like @Url");
    }

    [TestMethod]
    public void like_Sef_Attr() //TEntity ==  TSearch
    {
        var searchModel = new Input_like_Attr()
        {
            Url = "123",
        };
        var whereLambda = searchModel.CreateQueryConfig();
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
    public void like_self()
    {
        var searchModel = new Model_like()
        {
            Url = "123",
        };
        var whereLambda = new QueryConfig<Model_like, Model_like>(searchModel);

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
}