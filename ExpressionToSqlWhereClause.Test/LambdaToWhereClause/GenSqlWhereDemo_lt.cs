using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace ExpressionToSqlWhereClause.Test.LambdaToWhereClause
{
    public class model_lt
    {
        public long? Id { get; set; }
        public DateTime? DataCreatedAt { get; set; }
    }

    public class Input_lt_Attr
    {
        [SearchType(SearchType.Lt)] public long? Id { get; set; }
        [SearchType(SearchType.Lt)] public DateTime? DataCreatedAt { get; set; }
    }

    [TestClass]
    public class GenSqlWhereDemo_lt
    {
        [TestMethod]
        public void Test_lt()
        {
            var searchModel = new model_lt()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = new WhereLambda<Model_People, model_lt>();
            whereLambda.Search = searchModel;

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
}