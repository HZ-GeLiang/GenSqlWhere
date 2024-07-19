using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause
{
    public class model_ge
    {
        public long? Id { get; set; }
        public DateTime? DataCreatedAt { get; set; }
    }

    public class Input_ge_Attr
    {
        [SearchType(SearchType.Ge)] public long? Id { get; set; }
        [SearchType(SearchType.Ge)] public DateTime? DataCreatedAt { get; set; }
    }

    [TestClass]
    public class GenSqlWhereDemo_ge
    {
        [TestMethod]
        public void Test_ge()
        {
            var searchModel = new model_ge()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = new WhereLambda<Model_People, model_ge>(searchModel);

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
}
