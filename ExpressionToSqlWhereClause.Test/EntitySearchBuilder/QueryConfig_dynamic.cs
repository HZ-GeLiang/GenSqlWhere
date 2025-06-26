using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_dynamic
{
    [TestMethod]
    public void le_dynamic()
    {
        var searchModel = new
        {
            Id = 5,
            DataCreatedAt = DateTime.Parse("2021-8-8"),
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_dynamic));

        whereLambda[SearchType.Le] = new List<string>
        {
            nameof(searchModel.Id),
            nameof(searchModel.DataCreatedAt),
        };

        var searchCondition = whereLambda.ToExpression().ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "Id <= @Id And DataCreatedAt <= @DataCreatedAt");
        var dict = new Dictionary<string, object>
        {
            { "@Id", 5 },//取 domain 的类型
            { "@DataCreatedAt", searchModel.DataCreatedAt }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void le_dynamic_什么条件都没配()
    {
        var searchModel = new
        {
            Id = 5,
            DataCreatedAt = DateTime.Parse("2021-8-8"),
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_dynamic));

        var searchCondition = whereLambda.ToExpression().ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "");
        var dict = new Dictionary<string, object>();
        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }

    [TestMethod]
    public void le_dynamic_bool()
    {
        //int to bool  取决于 Convert.ChangeType(propertyValue, bool);

        {
            var searchModel = new
            {
                IsDel = 0,
            };

            var whereLambda = searchModel.CreateQueryConfig(default(Model_dynamic));
            whereLambda[SearchType.Eq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };

            var searchCondition = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", false },
            };
            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }

    [TestMethod]
    public void le_dynamic_int()
    {
        //int to bool  取决于 Convert.ChangeType(propertyValue, bool);

        {
            var searchModel = new
            {
                IsDel = 1,
            };

            var whereLambda = searchModel.CreateQueryConfig(default(Model_dynamic));
            whereLambda[SearchType.Eq] = new List<string>
            {
                nameof(searchModel.IsDel),
            };

            var searchCondition = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "IsDel = @IsDel");
            var dict = new Dictionary<string, object>
            {
                { "@IsDel", true },
            };
            CollectionAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }
}