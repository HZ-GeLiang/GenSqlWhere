using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_exception
{
    [TestMethod]
    public void expection1()
    {
        {
            //string的null 转 int
            var searchModel = new
            {
                Id = (string)null,
            };

            var whereLambda = searchModel.CreateQueryConfig(default(Model_expection1));

            whereLambda[SearchType.Eq] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var searchCondition = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, string.Empty);

            CollectionAssert.AreEqual(searchCondition.Parameters, new Dictionary<string, object>());
        }

        {
            //解决  "" 转 值类型(如int) 抛异常的问题
            var searchModel = new
            {
                Id = string.Empty,
            };

            var whereLambda = searchModel.CreateQueryConfig(default(Model_expection1));

            whereLambda[SearchType.Eq] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var searchCondition = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "");

            CollectionAssert.AreEqual(searchCondition.Parameters, new Dictionary<string, object>());
        }

        {
            //解决  "" 转 值类型(如int) 抛异常的问题
            var searchModel = new
            {
                Id = string.Empty,
            };

            var whereLambda = searchModel.CreateQueryConfig(default(Model_expection2));

            whereLambda[SearchType.Eq] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var searchCondition = whereLambda.ToExpression().ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "");

            CollectionAssert.AreEqual(searchCondition.Parameters, new Dictionary<string, object>());
        }
    }

    [TestMethod]
    public void sqlShouldBeEmpty()
    {
        var searchModel = new Model_lt()
        {
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));

        whereLambda[SearchType.Lt] = new List<string>
        {
            nameof(searchModel.Id),
            nameof(searchModel.DataCreatedAt),
        };

        var searchCondition = whereLambda.ToExpression().ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, string.Empty);
        CollectionAssert.AreEqual(searchCondition.Parameters, new Dictionary<string, object>(0));
    }
}