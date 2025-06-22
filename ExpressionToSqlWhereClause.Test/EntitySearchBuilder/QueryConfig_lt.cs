using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExpressionToSqlWhereClause.ExtensionMethods;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_lt
{
    [TestMethod]
    public void lt()
    {
        var searchModel = new Model_lt()
        {
            Id = 5,
            DataCreatedAt = DateTime.Parse("2021-8-8"),
        };

        var whereLambda = new QueryConfig<Model_People, Model_lt>(searchModel);

        whereLambda[SearchType.Lt] = new List<string>
        {
            nameof(searchModel.Id),
            nameof(searchModel.DataCreatedAt),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id < @Id And DataCreatedAt < @DataCreatedAt");
        var dict = new Dictionary<string, object>
        {
            { "@Id", 5 },//取 domain 的类型
            { "@DataCreatedAt", searchModel.DataCreatedAt }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }
}