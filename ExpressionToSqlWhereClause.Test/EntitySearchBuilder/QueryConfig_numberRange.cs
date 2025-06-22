using ExpressionToSqlWhereClause.EntitySearchBuilder;
using ExpressionToSqlWhereClause.ExtensionMethods;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Inputs;
using ExpressionToSqlWhereClause.Test.EntitySearchBuilder.Models;
using Infra.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.EntitySearchBuilder;

[TestClass]
public class QueryConfig_numberRange
{
    [TestMethod]
    public void numberRange()
    {
        {
            var searchModel = new Input_numberRange_Attr()
            {
                IdLeft = 3,
                IdRight = 9
            };
            var whereLambda = new QueryConfig<Model_People, Input_numberRange_Attr>(searchModel);

            whereLambda[SearchType.NumberRange] = new List<string>
            {
                nameof(searchModel.IdLeft),
                nameof(searchModel.IdRight),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Id >= @Id And Id <= @Id1");
            var dict = new Dictionary<string, object>
            {
                { "@Id",searchModel.IdLeft},
                { "@Id1",searchModel.IdRight},
            };

            DictionaryAssert.AreEqual(searchCondition.Parameters, dict);
        }

        {
            var searchModel = new Input_numberRange2()
            {
                Id = 5
            };
            var whereLambda = new QueryConfig<Model_People, Input_numberRange2>(searchModel);

            whereLambda[SearchType.NumberRange] = new List<string>
            {
                nameof(searchModel.Id),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Id >= @Id And Id <= @Id1");
            var dict = new Dictionary<string, object>
            {
                { "@Id",searchModel.Id},
                { "@Id1",searchModel.Id},
            };

            DictionaryAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }

    [TestMethod]
    public void numberRange_whereif()
    {
        {
            var searchModel = new Input_numberRange2()
            {
                Id = 5
            };
            var whereLambda = new QueryConfig<Model_People, Input_numberRange2>(searchModel);

            whereLambda[SearchType.NumberRange] = new List<string>
            {
                nameof(searchModel.Id),
            };

            whereLambda.WhereIf[nameof(searchModel.Id)] = a => a.Id > 6;// 满足 条件时 生效

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "");
            var dict = new Dictionary<string, object>
            {
            };

            DictionaryAssert.AreEqual(searchCondition.Parameters, dict);
        }
    }
}