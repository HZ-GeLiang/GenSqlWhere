using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class GenSqlWhereDemo_exception
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

            var whereLambda = searchModel.CreateWhereLambda(default(model_expection1));

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

            var whereLambda = searchModel.CreateWhereLambda(default(model_expection1));

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

            var whereLambda = searchModel.CreateWhereLambda(default(model_expection2));

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
        var searchModel = new model_lt()
        {
        };

        var whereLambda = new QueryConfig<Model_People, model_lt>(searchModel);

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

public class model_expection1
{
    public int Id { get; set; }
}

public class model_expection2
{
    public int? Id { get; set; }
}