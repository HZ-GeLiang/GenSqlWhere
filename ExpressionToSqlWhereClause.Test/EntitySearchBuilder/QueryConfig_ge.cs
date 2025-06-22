using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Modlesl;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_ge
{
    [TestMethod]
    public void ge()
    {
        var searchModel = new Model_ge()
        {
            Id = 5,
            DataCreatedAt = DateTime.Parse("2021-8-8"),
        };

        var whereLambda = new QueryConfig<Model_People, Model_ge>(searchModel);

        whereLambda[SearchType.Ge] = new List<string>
        {
            nameof(searchModel.Id),
            nameof(searchModel.DataCreatedAt),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id >= @Id And DataCreatedAt >= @DataCreatedAt");
        var dict = new Dictionary<string, object>
        {
            { "@Id", 5 },//取 domain 的类型
            { "@DataCreatedAt", searchModel.DataCreatedAt }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }
}