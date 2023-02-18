using System;
using System.Collections.Generic;
using ExpressionToSqlWhereClause.EntityConfig;
using ExpressionToSqlWhereClause.ExtensionMethod;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExpressionToSqlWhereClause.Test.WhereLambdaConfigToWhereClause
{
   
    [TestClass]
    public class GenSqlWhereDemo_gt
    {

        [TestMethod]
        public void Test_gt()
        {
            var searchModel = new model_gt()
            {
                Id = 5,
                DataCreatedAt = DateTime.Parse("2021-8-8"),
            };

            var whereLambda = new WhereLambda<Model_People, model_gt>();
            whereLambda.SearchModel = searchModel;

            whereLambda[SearchType.gt] = new List<string>
            {
                nameof(searchModel.Id),
                nameof(searchModel.DataCreatedAt),
            };

            var expression = whereLambda.ToExpression();
            var searchCondition = expression.ToWhereClause();

            Assert.AreEqual(searchCondition.WhereClause, "Id > @Id And DataCreatedAt > @DataCreatedAt");
            var dict = new Dictionary<string, object>
            {
                { "@Id", 5 },//取 domain 的类型
                { "@DataCreatedAt", searchModel.DataCreatedAt }
            };

           CollectionAssert.AreEqual(searchCondition.Parameters,  dict);
        }
    }

    public class model_gt
    {
        public long? Id { get; set; }
        public DateTime? DataCreatedAt { get; set; }
    }
    public class Input_gt_Attr
    {
        [SearchType(SearchType.gt)]
        public long? Id { get; set; }
        [SearchType(SearchType.gt)]
        public DateTime? DataCreatedAt { get; set; }
    }
}