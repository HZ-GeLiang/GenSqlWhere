using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_neq
{
    [TestMethod]
    public void neq()
    {
        var searchModel = new Model_neq()
        {
            IsDel = true,//todo://计划:添加当其他值为xx时,当前值才生效
        };

        var whereLambda = searchModel.CreateQueryConfig(default(Model_People));

        whereLambda[SearchType.Neq] = new List<string>
        {
            nameof(searchModel.IsDel),
        };

        var expression = whereLambda.ToExpression();
        var searchCondition = expression.ToWhereClause();

        Assert.AreEqual(searchCondition.WhereClause, "IsDel <> @IsDel");
        var dict = new Dictionary<string, object>
        {
            { "@IsDel", searchModel.IsDel }
        };

        CollectionAssert.AreEqual(searchCondition.Parameters, dict);
    }
}