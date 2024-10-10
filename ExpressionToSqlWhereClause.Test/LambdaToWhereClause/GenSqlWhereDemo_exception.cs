using ExpressionToSqlWhereClause.EntityConfig;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause;

[TestClass]
public class GenSqlWhereDemo_exception
{
    [TestMethod]
    public void Test_expection1()
    {
        {
            //string的null 转 int
            var searchModel = new
            {
                Id = (string)null,
            };

            var whereLambda = searchModel.CreateWhereLambda((model_expection1 _) => { });

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

            var whereLambda = searchModel.CreateWhereLambda((model_expection1 _) => { });

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

            var whereLambda = searchModel.CreateWhereLambda((model_expection2 _) => { });

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
    public void Test_sqlShouldBeEmpty()
    {
        var searchModel = new model_lt()
        {
        };

        var whereLambda = new WhereLambda<Model_People, model_lt>(searchModel);

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